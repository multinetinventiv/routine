namespace Routine.Interception;

public interface IChainLinkInterceptor<TContext> : IInterceptor<TContext>
    where TContext : InterceptionContext
{
    IChainLinkInterceptor<TContext> Next { get; set; }
}
