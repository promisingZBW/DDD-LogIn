using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UsersMgr.Domain.Entities;

namespace UserMgr.Infrastructure.configs
{
    internal class UserConfig : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            //这个属性的值只会包含数字、英文字母和英文符号时，可以为它配置 IsUnicode(false)
            //这里的a,b,c,x都只是表示一一对应关系
            builder.ToTable("T_users");
            //这段代码表示user有一个电话号码，而电话号码有一个号码
            builder.OwnsOne(x => x.PhoneNumber, 
                phonenumber => phonenumber.Property(a => a.Number).HasMaxLength(20).IsUnicode(false));
            //HasOne...WithOne..这段代码表示一对一关系，表示user对应UserAccessFail，UserAccessFail对应user
            //HasForeignKey<UserAccessFail>(b=>b.UserId);表示外键储存在UserAccessFail
            builder.HasOne(b => b.UserAccessFail).WithOne(c => c.User).HasForeignKey<UserAccessFail>(b=>b.UserId);

            //有一些user里面的不符合EFcore默认规则的属性或者成员变量也需要拿出来配置
            builder.Property("passwordHash").HasMaxLength(100).IsUnicode(false);
        }
    }
}
