using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using UsersMgr.Domain;
using UsersMgr.Domain.Entities;
using UsersMgr.Domain.ValueObjects;
using Zack.Infrastructure.EFCore;

namespace UserMgr.Infrastructure
{
    public class UserRepository : IUserRepository
    {
        private readonly UserDbContext dbContext;
        private readonly IDistributedCache distributedCache;//分布式缓存
        private readonly IMediator mediator;//mediator 可能被用作中介者对象，负责协调不同对象之间的通信和交互

        public UserRepository(UserDbContext dbContext, IDistributedCache distributedCache, IMediator mediator)
        {
            this.dbContext = dbContext;
            this.distributedCache = distributedCache;
            this.mediator = mediator;
        }

        public async Task AddNewLoginHistoryAsync(PhoneNumber phoneNumber, string message)
        {
            User? user = await FindOneAsync(phoneNumber);
            Guid? userId = null;
            if (user != null)
            {
                userId = user.UserId;
            }
            dbContext.UserLoginHistories.Add(new UserLoginHistory(userId, phoneNumber, message));
            
        }

        public async Task<User?> FindOneAsync(PhoneNumber phoneNumber)
        {
            //在 Entity Framework 中，Include() 用于在查询中启用“预加载”导航属性，允许在加载某个模型时，同时加载相关联的模型。
            //快速获取实体及其关联数据，简化数据处理逻辑。
            //Include(u=>u.UserAccessFail),这个代码是把这个u（user）的userAccessFail关联进来
            User? user = await dbContext.Users.Include(u => u.UserAccessFail).SingleOrDefaultAsync(ExpressionHelper.MakeEqual((User c) => c.PhoneNumber, phoneNumber));
            User? user2 = await dbContext.Users.Include(u => u.UserAccessFail).SingleOrDefaultAsync(a => a.PhoneNumber.Number == phoneNumber.Number && a.PhoneNumber.RegionNumber==phoneNumber.RegionNumber);
            return user2;//返回user和user2都行
            //User? user2 = await dbContext.Users.Include(u => u.UserAccessFail).SingleOrDefaultAsync(a => a.PhoneNumber == phoneNumber)注意不能这么写
            //因为SingleOrDefaultAsync并不会比较我们自己定义的PhoneNumber值对象，你得教他怎么比较
        }
        /*
        //另一种非异步的写法
        public Task<User?> FindOneAsync(PhoneNumber phoneNumber)
        {
            //在 Entity Framework Core 中，SingleOrDefault() 是 LINQ 查询方法之一，用于从数据库中检索满足特定条件的单个实体。
            //dbContext.Users.SingleOrDefault() 表示从 dbContext 数据上下文中的 Users 集合中检索单个用户实体。

            //using Zack.Infrastructure.EFCore;
            //MakeEqual可以用于比较俩个实体是否相等，这里比较传进来的phoneNumber和Users里面的每个User的phoneNumber是否相等
            User? user = dbContext.Users.SingleOrDefault(ExpressionHelper.MakeEqual((User c)=>c.PhoneNumber, phoneNumber));
            //User? user = dbContext.Users.SingleOrDefault(a=>a.PhoneNumber == phoneNumber);这么写也行
            //Task.FromResult() 是一个静态方法，用于创建一个已经完成并返回指定结果的 Task 对象。在异步编程中，Task 表示一个异步操作的结果或状态，并且可以用于表示异步方法的返回值。
            //通常情况下，使用 Task.FromResult() 用于将同步操作的结果包装在 Task 对象中，以便在异步方法中返回这个结果。这在需要将同步操作视为异步操作时非常有用，例如在异步方法中包装同步方法的返回值。
            return Task.FromResult(user);

        }*/



        public async Task<User?> FindOneAsync(Guid userId)
        {
            //比较users集合里的每个UserId是否和传进来的userId一样
            User? user = await dbContext.Users.Include(u => u.UserAccessFail).SingleOrDefaultAsync(u=>u.UserId == userId);
            return user;
        }

        public async Task<string?> FindPhoneNumberCodeAsync(PhoneNumber phoneNumber)
        {
            string key = $"phoneNumberCode_{phoneNumber.RegionNumber}_{phoneNumber.Number}";
            string? code = await distributedCache.GetStringAsync(key);
            distributedCache.Remove(key);//如果找到了验证码（即 code 不为 null），则通过 Remove 方法从分布式缓存中删除存储验证码的键（key）。这样做是为了保证验证码在被使用后被清除，以增强安全性和避免重复使用。
            return code;
        }

        public Task PublishEventAsync(UserAccessResultEvent _event)
        {
            return mediator.Publish(_event);
        }

        public Task SavePhoneNumberCodeAsync(PhoneNumber phoneNumber, string code)
        {
            //构建了一个唯一的键（key），这个键由电话号码的区号（RegionNumber）和号码（Number）组成，并且加上了前缀 "phoneNumberCode_"。
            string key =$"phoneNumberCode_{phoneNumber.RegionNumber}_{phoneNumber.Number}";
            //使用分布式缓存的 SetStringAsync 方法将验证码（code）存储在这个唯一的键（key）下，
            //设置了一个绝对过期时间为当前时间加上 5 分钟（TimeSpan.FromMinutes(5)）。这意味着验证码将在 5 分钟后过期并被自动清除。
            return distributedCache.SetStringAsync(key, code,new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow=TimeSpan.FromMinutes(5)});
        }
    }
}
