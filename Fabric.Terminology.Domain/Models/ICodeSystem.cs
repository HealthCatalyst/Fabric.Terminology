namespace Fabric.Terminology.Domain.Models
{
    using System;

    public interface ICodeSystem
    {
        /// <summary>
        /// Gets the code system's unique "code".
        /// </summary>
        /// <remarks>
        /// Maps to Terminology.ValueSetCode field CodeSystemCD Field
        /// </remarks>
        string Code { get; }

        string Name { get; }

        // TODO REMOVE
        string Version { get; }

        // TODO REMOVE
        //bool RetiredFLG { get; }

        // TODO REMOVE
        //DateTime RetiredDate { get; }
    }
}