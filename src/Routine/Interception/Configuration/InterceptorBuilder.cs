namespace Routine.Interception.Configuration;

public class InterceptorBuilder<TContext>
    where TContext : InterceptionContext
{
    public AsyncDecoratorInterceptor<TContext, TVariable> ByDecorating<TVariable>(Func<TVariable> beforeDelegate) => new(beforeDelegate);
    public AsyncDecoratorInterceptor<TContext, TVariable> ByDecorating<TVariable>(Func<TContext, TVariable> beforeDelegate) => new(beforeDelegate);
    public AsyncDecoratorInterceptor<TContext, TVariable> ByDecorating<TVariable>(Func<Task<TVariable>> beforeDelegate) => new(beforeDelegate);
    public AsyncDecoratorInterceptor<TContext, TVariable> ByDecorating<TVariable>(Func<TContext, Task<TVariable>> beforeDelegate) => new(beforeDelegate);

    public AsyncAroundInterceptor<TContext> Do() => new();

    public AsyncAroundInterceptor<TContext> Before(Action beforeDelegate) => Do().Before(beforeDelegate);
    public AsyncAroundInterceptor<TContext> Before(Action<TContext> beforeDelegate) => Do().Before(beforeDelegate);
    public AsyncAroundInterceptor<TContext> Before(Func<Task> beforeDelegate) => Do().Before(beforeDelegate);
    public AsyncAroundInterceptor<TContext> Before(Func<TContext, Task> beforeDelegate) => Do().Before(beforeDelegate);

    public AsyncAroundInterceptor<TContext> Success(Action successDelegate) => Do().Success(successDelegate);
    public AsyncAroundInterceptor<TContext> Success(Action<TContext> successDelegate) => Do().Success(successDelegate);
    public AsyncAroundInterceptor<TContext> Success(Func<Task> successDelegate) => Do().Success(successDelegate);
    public AsyncAroundInterceptor<TContext> Success(Func<TContext, Task> successDelegate) => Do().Success(successDelegate);

    public AsyncAroundInterceptor<TContext> Fail(Action failDelegate) => Do().Fail(failDelegate);
    public AsyncAroundInterceptor<TContext> Fail(Action<TContext> failDelegate) => Do().Fail(failDelegate);
    public AsyncAroundInterceptor<TContext> Fail(Func<Task> failDelegate) => Do().Fail(failDelegate);
    public AsyncAroundInterceptor<TContext> Fail(Func<TContext, Task> failDelegate) => Do().Fail(failDelegate);

    public AsyncAroundInterceptor<TContext> After(Action afterDelegate) => Do().After(afterDelegate);
    public AsyncAroundInterceptor<TContext> After(Action<TContext> afterDelegate) => Do().After(afterDelegate);
    public AsyncAroundInterceptor<TContext> After(Func<Task> afterDelegate) => Do().After(afterDelegate);
    public AsyncAroundInterceptor<TContext> After(Func<TContext, Task> afterDelegate) => Do().After(afterDelegate);
}
