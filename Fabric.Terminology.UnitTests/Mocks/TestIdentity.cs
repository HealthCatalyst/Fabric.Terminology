using System.Security.Claims;

namespace Fabric.Terminology.UnitTests.Mocks
{
    public class TestIdentity : ClaimsIdentity
    {
        public TestIdentity(params Claim[] claims) : base(claims, "testauthentication")
        { }
    }
}
