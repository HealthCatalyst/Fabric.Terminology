namespace Fabric.Terminology.API.Models
{
    // acquired from Fabric.Authorization.Domain
    public class Error
    {
        public string Code { get; set; }

        public string Message { get; set; }

        public string Target { get; set; }

        public Error[] Details { get; set; }

        public InnerError Innererror { get; set; }
    }

    // acquired from Fabric.Authorization.Domain
#pragma warning disable SA1402 // File may only contain a single class
    public class InnerError
#pragma warning restore SA1402 // File may only contain a single class
    {
        public string Code { get; set; }

        public InnerError Innererror { get; set; }
    }
}
