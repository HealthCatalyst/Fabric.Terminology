namespace Fabric.Terminology.TestsBase.Models
{
    using System.Security.Claims;

    public class TestIdentity : ClaimsIdentity
    {
        public TestIdentity(params Claim[] claims)
            : base(claims)
        {
        }
    }
}