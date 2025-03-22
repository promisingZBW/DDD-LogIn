using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using UserMgr.Infrastructure;
using UsersMgr.Domain;

namespace UserMgr.WebAPI.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly UserDomainService _userDomainService;

        public LoginController(UserDomainService userDomainService)
        {
            _userDomainService = userDomainService;
        }



        [HttpPost]
        //当某个标注了 [UnitOfWork] 的方法被调用时，它会为该方法自动创建一个 DbContext
        //它可能会自动开启数据库事务，在方法执行完成后提交或回滚事务。
        [UnitOfWork(typeof(UserDbContext))]//这里是因为CheckPassword中可能有修改数据的操作
        public async Task<IActionResult> LoginByPhoneAndPassword(LoginByPhoneAndPasswordRequest req)
        {

            if(req.password.Length<=3)
            {
                //在ASP.NET Core中，BadRequest() 是一个控制器的方法，用于返回一个表示 HTTP 400 Bad Request 状态码的响应
                return BadRequest("密码长度必须大于3");
            }
            var result = await _userDomainService.CheckPassword(req.phoneNumber, req.password);
            switch (result)
            {
                case UserAccessResult.OK:
                    return Ok("登录成功");
                case UserAccessResult.PasswordErrror:
                case UserAccessResult.NoPassword:
                case UserAccessResult.PhoneNumberNotFound:
                    return BadRequest("登录失败");
                case UserAccessResult.LockOut:
                    return BadRequest("账号被锁定");
                default:
                    throw new ApplicationException("未知值");//万一情况没有完全覆盖到就抛一个异常的未知值
            }
        }
    }
}
