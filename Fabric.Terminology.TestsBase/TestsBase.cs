using Xunit.Abstractions;

namespace Fabric.Terminology.TestsBase
{
    public abstract class TestsBase
    {
        protected TestsBase(ITestOutputHelper output)
        {
            this.Output = output;
        }

        protected ITestOutputHelper Output { get; }
    }
}