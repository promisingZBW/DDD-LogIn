using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UsersMgr.Domain.Entities
{
    public record UserAccessFail
    {
        public Guid Id { get; init; }//错误编号ID
        public User User { get; init; }
        public Guid UserId { get; init; } //这里定义了一个外键
        //外键的作用是让外键所在的表与主键所在的表相关联
        //比如客户表和订单表，客户表中包含主键Id，订单表中包含外键客户Id，建立俩者联系，一般外键id不能为空
        //这里的用户登录失败，用户ID不能为空

        private bool isLockOut;

        public DateTime?LockEnd { get; private set; }
        public int AccessFailCount { get; private set;}

        //在使用 Entity Framework Core（EF Core）时，创建一个无参数的空构造函数通常是为了使 EF Core 能够正确实例化实体对象。
        //EF Core 在查询数据库并将结果映射到实体对象时，需要一个无参数的构造函数来实例化对象。
        //在 EF Core 中，如果你有自定义的构造函数（带参数的构造函数），而没有无参数的构造函数，
        //那么 EF Core 在查询数据库时可能会遇到实例化对象的问题。
        private UserAccessFail() { }

        public UserAccessFail(User user)
        {
            this.User = user;
            this.Id = Guid.NewGuid();
        }

        public void Reset()
        {
            this.AccessFailCount = 0;
            this.LockEnd = null;
            this.isLockOut = false;
        }

        public void Fail()
        {
            this.AccessFailCount++;
            if(this.AccessFailCount >= 3)
            {
                this.LockEnd= DateTime.Now.AddMinutes(5);
                this.isLockOut= true;
            }
        }

        public bool IsLockOut()
        {
            if(this.isLockOut)
            {
                if(DateTime.Now>this.LockEnd)
                {
                    Reset();
                    return false;
                }
                else
                {
                    return true;
                }
            }
            else
            {
                return false;
            }
        }

    }
}
