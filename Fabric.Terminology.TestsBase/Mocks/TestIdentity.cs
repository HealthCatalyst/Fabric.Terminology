using System.Security.Claims;

namespace Fabric.Terminology.TestsBase.Mocks
{
    public class TestIdentity : ClaimsIdentity
    {
        public TestIdentity(params Claim[] claims) : base(claims, "testauthentication")
        { }
    }
}
