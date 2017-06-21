namespace Fabric.Terminology.Domain.Models
{
    public interface IValueSetCodeSystem
    {
        /// <summary>
        /// Gets the code system's unique "code".
        /// </summary>
        /// <remarks>
        /// Maps to Terminology.ValueSetCode field CodeSystemCD Field
        /// </remarks>
        string Code { get; }
        string Name { get; }
        string Version { get; }
    }
}