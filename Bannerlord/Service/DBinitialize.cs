using Bannerlord.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bannerlord.Service
{
    public class DBinitialize
    {
        public static void INIT(IServiceProvider serviceProvider)
        {
            var Context = new DBContext(
                serviceProvider.GetRequiredService<DbContextOptions<DBContext>>());
            Context.Database.EnsureCreated();

            if (Context.Files.Any())
            {
                return;
            }
            Context.Files.AddRange(DummyInit());
        }

        private static FilesModel DummyInit()
        {
            return new FilesModel();
        }
    }
}
