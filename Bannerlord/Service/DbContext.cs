using Bannerlord.Model;
using Microsoft.EntityFrameworkCore;

namespace Bannerlord.Service
{
    public class DBContext:DbContext
    {
        public DBContext(DbContextOptions<DBContext>options):base(options)
        {

        }
        public DbSet<FilesModel> Files { set; get; }

    }
}
