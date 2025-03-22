using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UsersMgr.Domain.Entities;
using UsersMgr.Domain.ValueObjects;

namespace UsersMgr.Domain
{
    public interface IUserRepository
    {
        public Task<User?> FindOneAsync(PhoneNumber phoneNumber);
        public Task<User?> FindOneAsync(Guid userId);

        public Task AddNewLoginHistoryAsync(PhoneNumber phoneNumber, string message);
        public Task SavePhoneNumberCodeAsync(PhoneNumber phoneNumber, string code);

        public Task<string?> FindPhoneNumberCodeAsync(PhoneNumber phoneNumber);

        public Task PublishEventAsync(UserAccessResultEvent _event);
    }
}
