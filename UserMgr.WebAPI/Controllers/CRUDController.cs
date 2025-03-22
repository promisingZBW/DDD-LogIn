using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using UserMgr.Infrastructure;
using UsersMgr.Domain;

namespace UserMgr.WebAPI.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class CRUDController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly UserDbContext _userDbContext;

        public CRUDController(IUserRepository userRepository, UserDbContext userDbContext)
        {
            _userRepository = userRepository;
            _userDbContext = userDbContext;
        }

        [HttpPost]
        [UnitOfWork(typeof(UserDbContext))]
        public async Task<IActionResult> AddNewUser(AddUserRequest req)
        {

            if(await _userRepository.FindOneAsync(req.phoneNumber)!=null)
            {
                return BadRequest("手机号已存在");
            }
            var user = new UsersMgr.Domain.Entities.User(req.phoneNumber);
            user.ChangePassword(req.password);
            _userDbContext.Users.Add(user);
            return Ok("完成");
        }
    }
}
