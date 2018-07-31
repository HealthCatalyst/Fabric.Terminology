namespace Fabric.Terminology.TestsBase.Models
{
    using System.Security.Claims;

    public class TestPrincipal : ClaimsPrincipal
    {
        public TestPrincipal(params Claim[] claims)
            : base(new TestIdentity(claims))
        {
        }
    }
}