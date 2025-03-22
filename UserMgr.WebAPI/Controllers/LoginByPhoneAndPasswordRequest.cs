using UsersMgr.Domain.ValueObjects;

namespace UserMgr.WebAPI.Controllers
{
    public record LoginByPhoneAndPasswordRequest(PhoneNumber phoneNumber,string password)
    {

    }
}
