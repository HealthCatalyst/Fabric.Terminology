namespace Fabric.Terminology.API.Models
{
    using NullGuard;

    // acquired from Fabric.Authorization.Domain
#pragma warning disable CA1716 // Identifiers should not match keywords
#pragma warning disable CA1819 // Properties should not return arrays
    public class Error
    {
        public string Code { get; set; }

        public string Message { get; set; }

        public string Target { get; set; }

        [AllowNull]
        public Error[] Details { get; set; }

        [AllowNull]
        public InnerError Innererror { get; set; }
    }
#pragma warning restore CA1819 // Properties should not return arrays
#pragma warning restore CA1716 // Identifiers should not match keywords
}
