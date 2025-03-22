using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UsersMgr.Domain.Entities;

namespace UserMgr.Infrastructure
{
    //DbContext，用于管理数据库操作,DbContext 是 EF Core 提供的一个类，负责与数据库通信。
    public class UserDbContext:DbContext
    {
        //只需要对根实体User生成Dbset，UserAccessFail则不用
        //DbSet<T> 是 EF Core 用来管理数据库表的对象：
        //DbSet<User> 代表 User 表。
        //DbSet<UserLoginHistory> 代表 UserLoginHistory 表。
        //这么做允许这些属性在代码中查询和操作数据库表
        public DbSet<User> Users {  get; set; }
        public DbSet<UserLoginHistory> UserLoginHistories { get; set; }

        //DbContextOptions<UserDbContext> 传递数据库配置（如连接字符串、数据库提供程序等）。
        //这个 : base(opt) 语法表示调用基类 DbContext 的构造函数，并将 opt 传递给它。让 EF Core 知道如何配置数据库连接。
        public UserDbContext(DbContextOptions<UserDbContext> opt) : base(opt) 
        {
            
        }

        //OnModelCreating(ModelBuilder modelBuilder) 是 EF Core 提供的一个方法，用于配置实体模型（如表结构、关系、索引等）
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //base.OnModelCreating(modelBuilder); 调用基类的 OnModelCreating，确保 EF Core 的默认行为不会丢失。
            base.OnModelCreating(modelBuilder);
            //让 EF Core 自动加载 当前程序集中的所有 EntityTypeConfiguration 配置类。
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }
    }
}
