namespace Fabric.Terminology.SqlServer.Caching
{
    using Fabric.Terminology.Domain.Models;

    public interface ICachingManagerFactory
    {
        IValueSetCachingManager<TResult> ResolveFor<TResult>() where TResult : class, IHaveValueSetGuid;
    }
}