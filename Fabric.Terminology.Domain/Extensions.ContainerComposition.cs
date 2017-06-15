using Fabric.Terminology.Domain.DependencyInjection;

namespace Fabric.Terminology.Domain
{
    public static partial class Extensions
    {
        public static TContainer ComposeFrom<TContainer, TComposition>(this TContainer container)
            where TComposition : class, IContainerComposition<TContainer>, new()
        {
            var composition = new TComposition();
            composition.Compose(container);
            return container;
        }
    }
}
