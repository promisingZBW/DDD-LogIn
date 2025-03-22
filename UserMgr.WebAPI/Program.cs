
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

//Filterɸѡ��������ASP.NET Core�ض���λ��ִ�������Զ���Ĵ���
//������һ�д�����������ȫ�ֹ�����
builder.Services.Configure<MvcOptions>(o => { o.Filters.Add<UnitOfWorkFilter>(); });
//ע�����ݿ�
builder.Services.AddDbContext<UserDbContext>(o =>
{
    o.UseSqlServer("Persist Security Info=False;Integrated Security=true;Initial Catalog=AdventureWorks;Server=DESKTOP-9E2KDIA\\SQLZBW;TrustServerCertificate=true");
});

builder.Services.AddDistributedMemoryCache();//���ӷֲ�ʽ����

//�� .NET ��̬ϵͳ�У�MediatR ��һ�����е� �н���ģʽ��Mediator Pattern�� ʵ�ֿ�
builder.Services.AddMediatR(Assembly.GetExecutingAssembly());
//����ע�뽫��ͬ�ӿں�ʵ�����໥ƥ�䣬Ҳ�п���һ���ӿ�ƥ�䲻ͬ��ʵ���࣬��Ӧ�Բ�ͬ������ע�������Ӧ�ò���£�Ҳ����˵Ӧ�ò���з����ƥ���ƴװ��
builder.Services.AddScoped<UserDomainService>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<ISmsCodeSender, MockSmsCodeSender>();


//���е�����ע�����Ӧ����������һ�д���ǰִ��
//һ��������Build()������Ӧ�ó�������ý���Ӧ�ã�Web��������������Ӧ�ó���׼���ý��ܺʹ������HTTP����
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
