using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System.Security.Principal;
using Bannerlord.Model;
using Bannerlord.Service;


namespace SoftsterUpload.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class FileController : ControllerBase
    {
        private IHostingEnvironment _hostingEnvironment;
        private WindowsIdentity LoginIdentity { get; }
        public DBContext Context { get; }

        public FileController(IHostingEnvironment environment, DBContext context)
        {
            _hostingEnvironment = environment;
            this.Context = context;
        }

        [HttpPost]
        [Route("Upload")]
        [RequestFormLimits(ValueLengthLimit = int.MaxValue, MultipartBodyLengthLimit = int.MaxValue)]
        public IActionResult Upload()
        {
            var FileUpload = Request.Form.Files.Count > 0 ? Request.Form.Files : null;
            List<FilesModel> result = new List<FilesModel>();
            if (FileUpload != null)
            {
                for (int i = 0; i < FileUpload.Count; i++)
                {
                    // fetch by key:
                    var File = FileUpload[i];
                    //save to DB
                    FilesModel file = new FilesModel();
                    file.Name = "ชื่อรูป";
                    string[] arrToken = File.FileName.Split('.');
                    file.Extension = "." + arrToken[arrToken.Length - 1]; //optional, not used
                    file.Length = File.Length;
                    //file.Width = 0;
                    //file.Height = 0;
                    file.ContentType = File.ContentType;
                    //file.Folder = "";
                    using (var ms = new MemoryStream())
                    {
                        File.CopyTo(ms);
                        var fileBytes = ms.ToArray();
                        string base64str = Convert.ToBase64String(fileBytes);
                        // act on the Base64 data
                        file.ContentBase64 = base64str;
                    }

                    //file.Content = Convert.ToBase64String(File); //   this.ConverToBytes(File);
                    file.Guid = Guid.NewGuid();
                    Context.Add(file);
                }
            }

            Context.SaveChanges();

            return Ok();
        }

        [HttpGet]
        [Route("getAllFiles")]
        public ActionResult<List<FilesModel>> AllFile()
        {
            List<FilesModel> RetModel = new List<FilesModel>();
            try
            {
                List<FilesModel> ret = Context.Files.ToList();
                if (ret != null)
                {
                 
                    return Ok(ret);
                }

            }
            catch (Exception ex)
            {
                return BadRequest("Folder not found : " + ex.Message);
            }

            return Ok(RetModel);
        }

        #region "Unuse"
        protected byte[] ConverToBytes(IFormFile file)
        {
            var length = file.OpenReadStream().Length;
            byte[] fileData = null;
            using (var binaryReader = new BinaryReader(file.OpenReadStream()))
            {
                fileData = binaryReader.ReadBytes(binaryReader.Read());
            }
            return fileData;
        }

        [HttpGet]
        [Route("GetUser")]
        public ActionResult<string> GetUser()
        {
            string UserName;
            if (string.IsNullOrEmpty(Request.HttpContext.User.Identity.Name))
            {
                UserName = "None";
            }
            else
            {
                IPrincipal user = Request.HttpContext.User;
                IIdentity identity = user.Identity;
                UserName = identity.Name.Substring(identity.Name.IndexOf(@"\") + 1);
            }
            return Ok(UserName);
        }


        [HttpGet]
        [Route("ggg")]
        public ActionResult<List<FilesModel>> ListFile(string username, string folder)
        {

            return BadRequest("Void Method");
        }

        [HttpGet]
        [Route("GetFolderList")]
        public ActionResult<List<string>> GetFolderList(string username)
        {
            if (string.IsNullOrEmpty(username)) return NotFound("User NotFound");
            List<string> Retval = new List<string>();
            try
            {
                DirectoryInfo di = new DirectoryInfo(Path.Combine(_hostingEnvironment.WebRootPath, username));
                if (di.Exists)
                {
                    foreach (DirectoryInfo item in di.GetDirectories().ToList())
                    {
                        Retval.Add(item.Name);
                    }
                }
                else
                {
                    return Ok();
                }

            }
            catch (Exception ex)
            {
                return NotFound("Wrong Path" + ex.Message);
            }

            return Ok(Retval);
        }

        private string MakeMD(string username)
        {
            string Folder = Path.Combine(username, DateTime.Today.ToString("yyyy_MM_dd"));
            string WebRootPath = Path.Combine(_hostingEnvironment.WebRootPath);

            if (!Directory.Exists(Path.Combine(WebRootPath, Folder)))
            {
                Directory.CreateDirectory(Path.Combine(WebRootPath, Folder));
            }
            return Folder = Path.Combine(WebRootPath, Folder);
        }

        public bool ByteArrayToFile(string fileName, byte[] byteArray)
        {
            try
            {
                using (var fs = new FileStream(fileName, FileMode.Create, FileAccess.Write))
                {
                    fs.Write(byteArray, 0, byteArray.Length);
                    return true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception caught in process: {0}", ex);
                return false;
            }
        }
        #endregion
    }


}
