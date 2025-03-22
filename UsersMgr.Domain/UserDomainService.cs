using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UsersMgr.Domain.Entities;
using UsersMgr.Domain.ValueObjects;

namespace UsersMgr.Domain
{
    public class UserDomainService
    {
        private IUserRepository _repository;
        private ISmsCodeSender _sender;

        public UserDomainService(IUserRepository repository, ISmsCodeSender sender)
        {
            _repository = repository;
            _sender = sender;
        }

        public void RestAccessFail(User user)
        {
            user.UserAccessFail.Reset();
        }

        public bool IsLockOut(User user)
        {
            //这里的user（父实例）要关联着UserAccessFail（子实例）
            //在使用 ORM 框架（如 Entity Framework）时，父实体（例如一个 User 对象）在从数据库中查询时，其关联的子实体（如用户的订单或角色等）不会自动加载。
            //这意味着，除非特意请求这些关联的数据，否则在查询时不会将它们读入内存。
            //所以传入的user实例要关联UserAccessFail，具体查看FindOneAsync（）方法,
            //如果没有像FindOneAsync（）方法中的那样处理，那么这里会报错，UserAccessFail为null
            return user.UserAccessFail.IsLockOut();
        }

        public void AccessFail(User user)
        {
            user.UserAccessFail.Fail();
        }


        public async Task<UserAccessResult> CheckPassword(PhoneNumber phoneNumber, string password)
        {
            UserAccessResult result;
            User user = await _repository.FindOneAsync(phoneNumber);
            if (user == null)
            {
                result = UserAccessResult.PhoneNumberNotFound;
            }
            else if (IsLockOut(user))
            {
                result = UserAccessResult.LockOut;
            }
            else if (user.HasPassword() == false)
            {
                result = UserAccessResult.NoPassword;
            }
            else if (user.CheckPassword(password))
            {
                result = UserAccessResult.OK;
            }
            else
            {
                return UserAccessResult.PasswordErrror;
            }

            if(user!=null)
            {
                if (result == UserAccessResult.OK)
                {
                    RestAccessFail(user);
                }
                else
                {
                    AccessFail(user);
                }
            }

            //这一步在发射事件
            await _repository.PublishEventAsync(new UserAccessResultEvent(phoneNumber,result));
            return result;
        }

        public async Task<CheckCodeResult> CheckPhoneNumberCodeAsync(PhoneNumber phoneNumber, string code)
        {
            User? user =await _repository.FindOneAsync(phoneNumber);
            if (user == null) 
            {
                    return CheckCodeResult.PhoneNumberNotFound;
            }
            else if(IsLockOut(user))
            {
                    return CheckCodeResult.Lockout;
            }
            
            string? codeInServer = await _repository.FindPhoneNumberCodeAsync(phoneNumber);
            if (codeInServer == null)
            {
                return CheckCodeResult.CodeError;
            }
            else if (codeInServer == code)
            {
                return CheckCodeResult.OK;
            }
            else
            {
                AccessFail(user);
                return CheckCodeResult.CodeError;
            }
        }


    }
}
