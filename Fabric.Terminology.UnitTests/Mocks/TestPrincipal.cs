using System.Security.Claims;

namespace Fabric.Terminology.UnitTests.Mocks
{
    public class TestPrincipal : ClaimsPrincipal
    {
        public TestPrincipal(params Claim[] claims) : base (new TestIdentity(claims))
        { }
    }
}
