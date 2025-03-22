using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UsersMgr.Domain.ValueObjects;

namespace UsersMgr.Domain.Entities
{
    public record UserLoginHistory:IAggregateRoot
    {
        public long Id { get; init; }
        public Guid? UserId { get; init; }//注意这里不要创建User对象，因为俩者不在用一个聚合内,作为一个独立的聚合，
                                          //他最好不引用其他聚合的对象，这样方便以后微服务的拆分
        public PhoneNumber PhoneNumber { get; init; }
        public DateTime CreateDateTime { get; init; }
        public string Message {  get; init; }//用于保存登录信息（失败/成功）

        private UserLoginHistory() { }

        public UserLoginHistory(Guid? userId, PhoneNumber phoneNumber, string message)
        {
            UserId = userId;
            PhoneNumber = phoneNumber;
            Message = message;
            CreateDateTime = DateTime.Now;
        }

    }
}
