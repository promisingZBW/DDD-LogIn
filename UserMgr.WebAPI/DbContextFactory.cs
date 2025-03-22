using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using UserMgr.Infrastructure;

namespace UserMgr.WebAPI
{
    public class DbContextFactory : IDesignTimeDbContextFactory<UserDbContext>
    {
        public UserDbContext CreateDbContext(string[] args)
        {
            DbContextOptionsBuilder<UserDbContext> builder = new DbContextOptionsBuilder<UserDbContext>();
            builder.UseSqlServer("Persist Security Info=False;Integrated Security=true;Initial Catalog=AdventureWorks;Server=DESKTOP-9E2KDIA\\SQLZBW;TrustServerCertificate=true");
            return new UserDbContext(builder.Options);
        }
    }
}

