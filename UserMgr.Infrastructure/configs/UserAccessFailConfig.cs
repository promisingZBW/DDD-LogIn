using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UsersMgr.Domain.Entities;

namespace UserMgr.Infrastructure.configs
{
    internal class UserAccessFailConfig : IEntityTypeConfiguration<UserAccessFail>
    {
        public void Configure(EntityTypeBuilder<UserAccessFail> builder)
        {
            builder.ToTable("T_UserAccessFail");
            builder.Property("isLockOut").IsRequired();//私有成员变量不会被自动映射到数据库表的列
            
        }
    }
}
