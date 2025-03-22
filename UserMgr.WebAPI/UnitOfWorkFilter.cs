using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace UserMgr.WebAPI
{
    //IAsyncActionFilter 接口是ASP.NET Core中定义的一个接口，用于实现异步动作过滤器
    //在 ASP.NET Core 中，Action Filter（动作过滤器） 允许在 控制器（Controller）动作方法（Action Method） 执行 之前（Before） 和 之后（After） 运行代码。
    public class UnitOfWorkFilter : IAsyncActionFilter
    {
        //该方法在 动作方法（Action）执行前后 运行。
        //ActionExecutingContext 是在执行控制器操作之前的过滤器阶段调用的。它包含有关即将执行的控制器操作的信息，例如控制器实例、动作方法描述、参数值等
        //next：是一个 ActionExecutionDelegate，用于执行下一个过滤器或动作方法。
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)//这里的ActionExecutionDelegate返回的是ActionExecutingContext
        {
            //查看上下文Context的下一步信息
            //next就是一个用来指向下一个操作筛选器的委托，
            //如果当前的操作筛选器是最后一个筛选器的话，next就会执行要执行的操作方法
            var result =await next();

            //result.Exception 表示在执行动作方法期间发生的任何异常。通过检查这个异常，
            //可以判断动作方法是否成功执行。在这段代码中，如果 result.Exception 不为 null，
            //意味着动作方法执行过程中出现了异常，此时代码中的逻辑是直接返回，不执行后续的操作
            if (result.Exception != null)//如果下一步信息中有任何异常信息则return
            {
                Console.WriteLine("UnitOfWorkFilter result.Exception != null");
                return;
            }
            //从 context 中获取当前动作的描述信息
            var actionDesc = context.ActionDescriptor as ControllerActionDescriptor;
            if (actionDesc == null)
            {
                Console.WriteLine("UnitOfWorkFilter actionDesc == null");
                return;
            }

            //MethodInfo只有ControllerActionDescriptor才有，所以把ActionDescriptor转化为ControllerActionDescriptor，而ControllerActionDescriptor本就是继承于ActionDescriptor
            //Attribute是特征的意思，就段代码是查看动作方法中有没有UnitOfWorkAttribute标记的动作方法，标记的了才savechanges
            var uowAttr = actionDesc.MethodInfo.GetCustomAttribute<UnitOfWorkAttribute>();

            if (uowAttr == null)
            {
                Console.WriteLine("UnitOfWorkFilter uowAttr = null");
                return;
            }
            foreach(var dbCtxType in uowAttr.DbContextTypes)
            {
                var dbCtx = context.HttpContext.RequestServices.GetService(dbCtxType) as DbContext;//通过依赖注入（DI）从请求的服务提供者中获取对应的 DbContext 实例。
                if (dbCtx != null)
                {
                    Console.WriteLine("UnitOfWorkFilter has executed.");
                    await dbCtx.SaveChangesAsync();
                }
            }
        }
    }
}
