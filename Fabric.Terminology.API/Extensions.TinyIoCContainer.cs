namespace Fabric.Terminology.API
{
    using Fabric.Terminology.Domain.DependencyInjection;
    using Nancy.TinyIoc;

    /// <summary>
    /// Extension methods for Nancy's <see cref="TinyIoCContainer"/>
    /// </summary>
    public static partial class Extensions
    {
        public static TinyIoCContainer ComposeFrom<TComposition>(this TinyIoCContainer container)
            where TComposition : class, IContainerComposition<TinyIoCContainer>, new()
        {
            var composition = new TComposition();
            composition.Compose(container);
            return container;
        }
    }
}
