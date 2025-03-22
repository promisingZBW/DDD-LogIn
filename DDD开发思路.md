

# 领域层

## 实体

注：该说明书的例子皆为DDD实战用户登录

**聚合根：**

创建一个IAggregateRoot.cs接口来标识聚合根，聚合根要实现这个接口，目的只是明确表明谁是聚合根

聚合根（父实例），聚合中的其他实例（子对象）

聚合根（例：user）

​	需要实现：

​		1.创建主键，聚合根的 ID 一般对应于数据库表中的主键，这样可以轻松地查找、更新或删除相应的记录。通常为Guid类型。**GUID** 是“全局唯一标识符”（Globally Unique Identifier）的缩写。

​		2.需要创建聚合中的其它实例，子对象。如，用户的手机号，密码，登录成功或失败的信息...

​		3.需要创建一个无参数的空构造函数通常是为了使 EF Core 能够正确实例化实体对象。然后再创建一个自己使用的构造函数。前者private，后者public。

​		4.实现基本的业务逻辑，主要是有关手机号，密码等子对象的业务，如是否有密码？更改密码，更改手机号，密码是否正确等。



**聚合中的其他实例，但需要自己单独创建的，比较复杂的类：**

​	需要实现：（如UserAccessFail）

​		1.初始化自己的主键，但和聚合根的主键有区别。

​		2.初始化聚合根对象。

​		3.初始化一个外键，这里的外键就是聚合根的主键（如UserId）。

​		4.需要创建一个无参数的空构造函数，和供自己使用的构造函数。

​		5.实现基本的业务逻辑。如登录失败，用户锁定，重置用户状态等。



**注：**

​	1.把user和UserAccessFail设计为同一个聚合，并且把User设置为聚合根，因为用户登录失败次数过多不是用户这个实体的常见特征，所以俩者分开。但是登录失败，我们不会直接操作AccessFail，还是会通过User进行操作。

​	2.一个项目中可能有多个聚合和聚合根，不同的聚合之间不要互相直接引用/初始化对方的聚合根对象，而是用聚合根的主键来进行相互联系。如UserLoginHistory和User是俩个聚合，在UserLoginHistory中不要初始化**User**，而是初始化public Guid? **UserId** { get; init; }。便于微服务/职责的拆分。



### 值对象

​	值对象（Value Objects）是一种特殊类型的对象，其主要特征是其身份由其属性值决定。值对象通常用于表示具有明确定义且不可变性质的概念，如日期、时间、货币金额等，它们在整个生命周期中保持不变。



## 领域事件

​	"事件"（Event）是指系统内部或外部发生的重要事情或发生的动作，通常会触发相应的反应或处理。

​	在DDD实战项目中，有一个用户登录结果事件类UserAccessResultEvent。在领域层中，它只是一个实现INotification的接口。（INotification是通知类型接口）它的用处是，当后面用户真的登录成功或者失败后，就会有一个函数（PublishEventAsync（））发布结果

​	然后待结果发布后，就会又有一个如下的类来接收这个结果并处理这个通知事件

```c#
public class UserAccessResultEventHandler:INotificationHandler<UserAccessResultEvent>
```



## 领域服务

​	**组装接口，体现业务逻辑**

​	1.初始化一些类的实例，这样可以调用这些类的方法，虽然这些类中的方法在代码初期，或者更准确的说，在领域层中，还没到具体实现某些方法的地步。

​	2.组装接口

如：之前在User类中有重置方法，就要组装这个接口（没实现就还是接口阶段）

```c#
public void RestAccessFail(User user)
{
	user.UserAccessFail.Reset();
}
```



## 其他

​	在领域服务中还有一些类，这些类用于简单的逻辑功能的实现，或者单纯是一些简单功能的接口。

​	如检查验证码的结果（枚举类）

```c#
    public enum CheckCodeResult
    {
        OK,PhoneNumberNotFound,Lockout,CodeError
    }
```

又或者是有关数据库操作的一些功能的接口总和类

```c#
    public interface IUserRepository
    {
        public Task<User?> FindOneAsync(PhoneNumber phoneNumber);
        public Task<User?> FindOneAsync(Guid userId);

        public Task AddNewLoginHistoryAsync(PhoneNumber phoneNumber, string message);
        public Task SavePhoneNumberCodeAsync(PhoneNumber phoneNumber, string code);

        public Task<string?> FindPhoneNumberCodeAsync(PhoneNumber phoneNumber);

        public Task PublishEventAsync(UserAccessResultEvent _event);
    }
```



### 防腐层

避免外部系统的变化或不一致影响到内部设计

#### **应用场景**（DDD的这个项目体现的不明显）

- **遗留系统集成**：在将旧系统迁移到新的微服务架构时，可以使用防腐层保护新系统不受遗留系统设计的影响。
- **跨系统通信**：当多个微服务需要协同工作，但又不希望彼此共享相同的领域模型时，防腐层可以作为桥梁，处理两个或多个领域模型之间的交互。
- **不同数据模型**：在拦截和转换来自不同来源（如数据库、API、消息队列等）的数据时，可以使用防腐层来处理模型转换和适配。

如发送验证码的接口类

```c#
    public interface ISmsCodeSender
    {
        Task SendAsync(PhoneNumber phoneNumber, string code);
    }
```







# 基础设施层

基础设施层引用领域层



## 配置Configs

负责将类中的实体映射到数据库中

1.设置表名

2.将私有成员变量映射到数据库表的列，因为私有成员变量不会被自动映射到数据库表的列

3.其中包括ownsone、hasone、 withone等实体间关系配置方法

​	`ownsone`用于定义**拥有型关系**（ownership relationship），这里的拥有型关系表示一个实体包含（拥有）另一个实体（通常称为“被拥有实体”），且被拥有实体是其生命周期和性质的一部分。	

​	`HasOne` 表示一个实体与另一个主（principal）实体之间的关系

​	**`WithOne`**：用于配置与主实体（拥有实体）的一对一关系。它通常与 `HasOne` 配合使用

具体使用方法可以问Ai，不过多介绍



注：不符合EFcore默认规则的属性或者成员变量也需要拿出来配置

如：不可访问的属性、某些数据类型（如 `sbyte`, `char`, `object`, 复杂类型等）、使用了 `null` 可以的类型、不能映射的静态成员static....等



## Migrations工具类

`1.`UserDbContext

​	DbContext 是 EF Core 提供的一个类，负责与数据库通信。

​	**对聚合根创建表，通过聚合根查询和操作数据库表**

```c#
//只需要对根实体User生成Dbset，UserAccessFail则不用
public DbSet<User> Users {  get; set; }
public DbSet<UserLoginHistory> UserLoginHistories { get; set; }
```

​	创建构造函数

```c#
//DbContextOptions<UserDbContext> 传递数据库配置（如连接字符串、数据库提供程序等）。
//这个 : base(opt) 语法表示调用基类 DbContext 的构造函数，并将 opt 传递给它。让 EF Core 知道如何配置数据库连接。
public UserDbContext(DbContextOptions<UserDbContext> opt) : base(opt) {}
```



`2.`DbContextFactory.cs类

​	**执行迁移、与数据库交互**

​	实现接口IDesignTimeDbContextFactory<UserDbContext>

```c#
public class DbContextFactory : IDesignTimeDbContextFactory<UserDbContext>
```

​	连接数据库



注：具体细节代码看DDD文件



`3.`创建UserRepository.cs类

**注入Dbcontext类、分布式缓存、中介者对象用于实现具体存储功能**

```c#
private readonly UserDbContext dbContext;
private readonly IDistributedCache distributedCache;//分布式缓存
private readonly IMediator mediator;//mediator 可能被用作中介者对象，负责协调不同对象之间的通信和交互

public UserRepository(UserDbContext dbContext, IDistributedCache distributedCache, IMediator mediator)
{
	this.dbContext = dbContext;
	this.distributedCache = distributedCache;
	this.mediator = mediator;
}
```

实现IUserRepository.cs类中的接口





# Web应用层

注：引用领域层和基础设施层

注：具体代码查看DDD代码实现



## 消息处理类

对之前发出的通知进行处理和实现的类

```c#
public class UserAccessResultEventHandler : INotificationHandler<UserAccessResultEvent>
```



## 工作单元

1.工作单元特征类

```c#
public class UnitOfWorkAttribute: Attribute //这个类继承自 System.Attribute，表明它是一个特性，可以通过 [UnitOfWork] 这样的注解来使用。
```

2.工作单元过滤器类

```c#
public class UnitOfWorkFilter : IAsyncActionFilter
```

这里实现的是动作过滤器，可以做到所有标记了[UnitOfWork]的WebApi请求都执行该动作过滤器



## 主程序类Program.cs

1.设置全局过滤器

2.注入数据库

3.增加分布式缓存

4.依赖注入将不同接口和实现类相互匹配

5.调用swagger调试





## 控制器类Controllers.cs

用于进行最后功能的拼装，发出post，get的http请求