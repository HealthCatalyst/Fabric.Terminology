namespace Fabric.Terminology.Domain.Visitors
{
    public interface IVisitor<in T>
    {
        void Visit(T item);
    }
}