using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UsersMgr.Domain.ValueObjects;
using Zack.Commons;

namespace UsersMgr.Domain.Entities
{
    //把user和UserAccessFail设计为同一个聚合，并且把User设置为聚合根，
    //因为用户登录失败次数过多不是用户这个实体的常见特征，所以俩者分开。但是登录失败，
    //我们不会直接操作AccessFail，还是会通过User进行操作
    public class User: IAggregateRoot  //这里的“: IAggregateRoot ”主要是想表示User是聚合根，用User来与外界交互
    {
        public Guid UserId { get; init; }//主键
        public PhoneNumber PhoneNumber { get; private set; }//允许类内部的方法进行修改

        public string? passwordHash; //储存用户登录的哈希码

        public UserAccessFail UserAccessFail { get; private set; }

        private User() { }

        public User(PhoneNumber phoneNumber)
        {
            PhoneNumber = phoneNumber;
            this.UserId=Guid.NewGuid();
            this.UserAccessFail=new UserAccessFail(this);//新生成一个用户的登录错误信息表
        }

        public bool HasPassword()
        {
            //! 取反，如果为空有密码，没有则没密码
            return !string.IsNullOrEmpty(this.passwordHash);
        }

        public void ChangePassword(string Password)
        {
            if(Password.Length <=3)
            {
                throw new ArgumentOutOfRangeException("密码长度必须大于3");
            }
            this.passwordHash = HashHelper.ComputeMd5Hash(Password);//安装zack.commons包
        }

        public bool CheckPassword(string Password)
        {
            //返回判断数据库保存的哈希码和现在传进来转化后的哈希码的值是否一样
            return this.passwordHash == HashHelper.ComputeMd5Hash(Password);
        }

        public void ChangePhoneNumber(PhoneNumber PhoneNumber)
        {
            this.PhoneNumber = PhoneNumber;
        }
    }
}
