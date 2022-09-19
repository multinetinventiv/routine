namespace Routine.Interception.Configuration;

public class InterceptorBuilder<TContext>
    where TContext : InterceptionContext
{
    public DecoratorInterceptor<TContext, TVariable> ByDecorating<TVariable>(Func<TVariable> beforeDelegate) => new(beforeDelegate);
    public DecoratorInterceptor<TContext, TVariable> ByDecorating<TVariable>(Func<TContext, TVariable> beforeDelegate) => new(beforeDelegate);
    public DecoratorInterceptor<TContext, TVariable> ByDecorating<TVariable>(Func<Task<TVariable>> beforeDelegate) => new(beforeDelegate);
    public DecoratorInterceptor<TContext, TVariable> ByDecorating<TVariable>(Func<TContext, Task<TVariable>> beforeDelegate) => new(beforeDelegate);

    public AroundInterceptor<TContext> Do() => new();

    public AroundInterceptor<TContext> Before(Action beforeDelegate) => Do().Before(beforeDelegate);
    public AroundInterceptor<TContext> Before(Action<TContext> beforeDelegate) => Do().Before(beforeDelegate);
    public AroundInterceptor<TContext> Before(Func<Task> beforeDelegate) => Do().Before(beforeDelegate);
    public AroundInterceptor<TContext> Before(Func<TContext, Task> beforeDelegate) => Do().Before(beforeDelegate);

    public AroundInterceptor<TContext> Success(Action successDelegate) => Do().Success(successDelegate);
    public AroundInterceptor<TContext> Success(Action<TContext> successDelegate) => Do().Success(successDelegate);
    public AroundInterceptor<TContext> Success(Func<Task> successDelegate) => Do().Success(successDelegate);
    public AroundInterceptor<TContext> Success(Func<TContext, Task> successDelegate) => Do().Success(successDelegate);

    public AroundInterceptor<TContext> Fail(Action failDelegate) => Do().Fail(failDelegate);
    public AroundInterceptor<TContext> Fail(Action<TContext> failDelegate) => Do().Fail(failDelegate);
    public AroundInterceptor<TContext> Fail(Func<Task> failDelegate) => Do().Fail(failDelegate);
    public AroundInterceptor<TContext> Fail(Func<TContext, Task> failDelegate) => Do().Fail(failDelegate);

    public AroundInterceptor<TContext> After(Action afterDelegate) => Do().After(afterDelegate);
    public AroundInterceptor<TContext> After(Action<TContext> afterDelegate) => Do().After(afterDelegate);
    public AroundInterceptor<TContext> After(Func<Task> afterDelegate) => Do().After(afterDelegate);
    public AroundInterceptor<TContext> After(Func<TContext, Task> afterDelegate) => Do().After(afterDelegate);
}
