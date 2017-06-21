using System.Security.Claims;

namespace Fabric.Terminology.TestsBase.Mocks
{
    public class TestPrincipal : ClaimsPrincipal
    {
        public TestPrincipal(params Claim[] claims) : base (new TestIdentity(claims))
        { }
    }
}
