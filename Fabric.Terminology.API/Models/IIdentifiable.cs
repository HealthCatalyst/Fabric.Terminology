namespace Fabric.Terminology.API.Models
{
    /// <summary>
    /// Represents an entity with a set (unique) identifier
    /// </summary>
    /// <remarks>
    /// acquired from Fabric.Authorization.Domain
    /// </remarks>
    public interface IIdentifiable
    {
        /// <summary>
        /// Gets the identifier
        /// </summary>
        string Identifier { get; }
    }
}