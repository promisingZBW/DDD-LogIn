using MediatR;
using System.Threading;
using UserMgr.Infrastructure;
using UsersMgr.Domain;

namespace UserMgr.WebAPI
{
    //UserAccessResultEvent表示此消息处理者要处理的消息类型
    public class UserAccessResultEventHandler : INotificationHandler<UserAccessResultEvent>
    {

        private readonly IUserRepository userRepository;
        private readonly UserDbContext userDbContext;

        public UserAccessResultEventHandler(IUserRepository userRepository, UserDbContext userDbContext)
        {
            this.userRepository = userRepository;
            this.userDbContext = userDbContext;
        }

        public async Task Handle(UserAccessResultEvent notification, CancellationToken cancellationToken)
        {
            await userRepository.AddNewLoginHistoryAsync(notification.PhoneNumber, $"登录结果是{notification.UserAccessResult}");
            Console.WriteLine("UserAccessResultEventHandler has executed.");
            await userDbContext.SaveChangesAsync();
        }
    }
}
