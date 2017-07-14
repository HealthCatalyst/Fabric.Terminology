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
}
