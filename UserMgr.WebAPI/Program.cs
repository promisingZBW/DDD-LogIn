
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using UserMgr.Infrastructure;
using UserMgr.WebAPI;
using UsersMgr.Domain;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

//Filter筛选器可以在ASP.NET Core特定的位置执行我们自定义的代码
//下面这一行代码是在设置全局过滤器
builder.Services.Configure<MvcOptions>(o => { o.Filters.Add<UnitOfWorkFilter>(); });
//注入数据库
builder.Services.AddDbContext<UserDbContext>(o =>
{
    o.UseSqlServer("Persist Security Info=False;Integrated Security=true;Initial Catalog=AdventureWorks;Server=DESKTOP-9E2KDIA\\SQLZBW;TrustServerCertificate=true");
});

builder.Services.AddDistributedMemoryCache();//增加分布式缓存

//在 .NET 生态系统中，MediatR 是一个流行的 中介者模式（Mediator Pattern） 实现库
builder.Services.AddMediatR(Assembly.GetExecutingAssembly());
//依赖注入将不同接口和实现类相互匹配，也有可能一个接口匹配不同的实现类，以应对不同的需求（注册服务是应用层的事，也就是说应用层进行服务的匹配和拼装）
builder.Services.AddScoped<UserDomainService>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<ISmsCodeSender, MockSmsCodeSender>();


//所有的依赖注入服务都应该在下面这一行代码前执行
//一旦调用了Build()方法，应用程序的配置将被应用，Web主机将启动，并应用程序将准备好接受和处理传入的HTTP请求。
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/openapi/v1.json", "v1");
    });
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
