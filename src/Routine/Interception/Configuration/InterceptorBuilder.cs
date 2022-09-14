namespace Routine.Interception.Configuration;

public class InterceptorBuilder<TContext>
    where TContext : InterceptionContext
{
    public DecoratorInterceptor<TContext, TVariable> ByDecorating<TVariable>(Func<TVariable> beforeDelegate) => new(beforeDelegate);
    public DecoratorInterceptor<TContext, TVariable> ByDecorating<TVariable>(Func<TContext, TVariable> beforeDelegate) => new(beforeDelegate);

    public AsyncDecoratorInterceptor<TContext, TVariable> ByDecoratingAsync<TVariable>(Func<TVariable> beforeDelegate) => new(beforeDelegate);
    public AsyncDecoratorInterceptor<TContext, TVariable> ByDecoratingAsync<TVariable>(Func<TContext, TVariable> beforeDelegate) => new(beforeDelegate);
    public AsyncDecoratorInterceptor<TContext, TVariable> ByDecoratingAsync<TVariable>(Func<Task<TVariable>> beforeDelegate) => new(beforeDelegate);
    public AsyncDecoratorInterceptor<TContext, TVariable> ByDecoratingAsync<TVariable>(Func<TContext, Task<TVariable>> beforeDelegate) => new(beforeDelegate);

    public AroundInterceptor<TContext> Do() => new();
    public AsyncAroundInterceptor<TContext> DoAsync() => new();

    //first level facade
    public AroundInterceptor<TContext> Before(Action beforeDelegate) => Do().Before(beforeDelegate);
    public AroundInterceptor<TContext> Before(Action<TContext> beforeDelegate) => Do().Before(beforeDelegate);

    public AroundInterceptor<TContext> Success(Action successDelegate) => Do().Success(successDelegate);
    public AroundInterceptor<TContext> Success(Action<TContext> successDelegate) => Do().Success(successDelegate);

    public AroundInterceptor<TContext> Fail(Action failDelegate) => Do().Fail(failDelegate);
    public AroundInterceptor<TContext> Fail(Action<TContext> failDelegate) => Do().Fail(failDelegate);

    public AroundInterceptor<TContext> After(Action afterDelegate) => Do().After(afterDelegate);
    public AroundInterceptor<TContext> After(Action<TContext> afterDelegate) => Do().After(afterDelegate);

    public AsyncAroundInterceptor<TContext> BeforeAsync(Action beforeDelegate) => DoAsync().Before(beforeDelegate);
    public AsyncAroundInterceptor<TContext> BeforeAsync(Action<TContext> beforeDelegate) => DoAsync().Before(beforeDelegate);
    public AsyncAroundInterceptor<TContext> BeforeAsync(Func<Task> beforeDelegate) => DoAsync().Before(beforeDelegate);
    public AsyncAroundInterceptor<TContext> BeforeAsync(Func<TContext, Task> beforeDelegate) => DoAsync().Before(beforeDelegate);

    public AsyncAroundInterceptor<TContext> SuccessAsync(Action successDelegate) => DoAsync().Success(successDelegate);
    public AsyncAroundInterceptor<TContext> SuccessAsync(Action<TContext> successDelegate) => DoAsync().Success(successDelegate);
    public AsyncAroundInterceptor<TContext> SuccessAsync(Func<Task> successDelegate) => DoAsync().Success(successDelegate);
    public AsyncAroundInterceptor<TContext> SuccessAsync(Func<TContext, Task> successDelegate) => DoAsync().Success(successDelegate);

    public AsyncAroundInterceptor<TContext> FailAsync(Action failDelegate) => DoAsync().Fail(failDelegate);
    public AsyncAroundInterceptor<TContext> FailAsync(Action<TContext> failDelegate) => DoAsync().Fail(failDelegate);
    public AsyncAroundInterceptor<TContext> FailAsync(Func<Task> failDelegate) => DoAsync().Fail(failDelegate);
    public AsyncAroundInterceptor<TContext> FailAsync(Func<TContext, Task> failDelegate) => DoAsync().Fail(failDelegate);

    public AsyncAroundInterceptor<TContext> AfterAsync(Action afterDelegate) => DoAsync().After(afterDelegate);
    public AsyncAroundInterceptor<TContext> AfterAsync(Action<TContext> afterDelegate) => DoAsync().After(afterDelegate);
    public AsyncAroundInterceptor<TContext> AfterAsync(Func<Task> afterDelegate) => DoAsync().After(afterDelegate);
    public AsyncAroundInterceptor<TContext> AfterAsync(Func<TContext, Task> afterDelegate) => DoAsync().After(afterDelegate);
}
