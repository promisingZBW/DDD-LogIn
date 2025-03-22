using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserMgr.Infrastructure
{
    public class DbContextFactory : IDesignTimeDbContextFactory<UserDbContext>
    {
        public UserDbContext CreateDbContext(string[] args)
        {
            var builder = new DbContextOptionsBuilder<UserDbContext>();
            builder.UseSqlServer("Persist Security Info=False;Integrated Security=true;Initial Catalog=AdventureWorks;Server=DESKTOP-9E2KDIA\\SQLZBW;TrustServerCertificate=true");
            //通过使用 builder.Options，可以实现将配置好的数据库连接选项和其他上下文选项传递给 DbContext，
            //以便在运行时实例化 DbContext 并与数据库进行通信。
            return new UserDbContext(builder.Options);
        }
    }
}
