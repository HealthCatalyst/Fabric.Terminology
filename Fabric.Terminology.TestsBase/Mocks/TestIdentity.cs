namespace Fabric.Terminology.TestsBase.Mocks
{
    using System.Security.Claims;

    public class TestIdentity : ClaimsIdentity
    {
        public TestIdentity(params Claim[] claims)
            : base(claims, "testauthentication")
        {
        }
    }
}
