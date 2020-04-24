using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Bannerlord.Model
{
    public class FilesModel
    {
        public string Name { set; get; }
        public string ContentType { set; get; }
        public long Length { set; get; }

        public string Extension { set; get; }
        public string ContentBase64 { set; get; }
        [Key]
        public Guid Guid { set; get; }
       
    }
}
