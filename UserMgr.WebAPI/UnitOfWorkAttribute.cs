namespace UserMgr.WebAPI
{
    [AttributeUsage(AttributeTargets.Class
        | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]

    //在这个特定的代码中，UnitOfWorkAttribute 自定义特性可能被用于标记方法，以指示该方法应该在一个单元操作（Unit of Work）中执行。
    //单元操作是指作为一组相关操作的逻辑单元，要么全部成功完成，要么全部失败回滚。

    //当开发者在某个方法上应用 UnitOfWorkAttribute 特性时，可能会有一些处理逻辑，比如在方法执行前启动一个事务，
    //在方法执行完成后提交事务或回滚事务。这有助于确保一系列操作要么全部成功，要么全部失败，保持数据的一致性。
    public class UnitOfWorkAttribute: Attribute //这个类继承自 System.Attribute，表明它是一个特性，可以通过 [UnitOfWork] 这样的注解来使用。
    {
        //指定数据库类型
        public Type[] DbContextTypes { get; init; } // init表示只能通过构造函数来赋值

        //使用 params 关键字允许你在调用方法时可以传递不定数量的参数，而不需要显式地将这些参数封装在数组中
        public UnitOfWorkAttribute(params Type[] DbContextTypes)
        {
            this.DbContextTypes = DbContextTypes;
        }
    }
}
//如果一个控制器上标注了UnitOfWorkAttribute，那么这个控制器中所有的方法都会在执行结束后自动提交工作单元，
//我们也可以把UnitOfWorkAttribute添加到控制器的方法上。因为一个微服务中可能有多个上下文，所以我们通过DbContextTypes来指定工作单元结束后程序自动调用哪些上下文的SaveChangesAsync方法，
//DbContextTypes属性用来指定上下文的类型