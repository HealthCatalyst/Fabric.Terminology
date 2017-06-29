namespace Fabric.Terminology.API.Models
{
    // acquired from Fabric.Authorization.Domain
    public class InnerError
    {
        public string Code { get; set; }

        public InnerError Innererror { get; set; }
    }
}