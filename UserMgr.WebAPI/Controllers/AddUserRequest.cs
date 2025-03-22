using UsersMgr.Domain.ValueObjects;

namespace UserMgr.WebAPI.Controllers
{
        public record AddUserRequest(PhoneNumber phoneNumber, string password);
}
