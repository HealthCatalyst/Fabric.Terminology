namespace Fabric.Terminology.Domain.DependencyInjection
{
    public interface IContainerComposition<in TContainer>
    {
        void Compose(TContainer container);
    }
}