using Xunit.Abstractions;

namespace Fabric.Terminology.TestsBase
{
    public abstract class OutputTestBase
    {
        protected OutputTestBase(ITestOutputHelper output)
        {
            this.Output = output;
            Profiler = new TestProfiler(output);
        }

        protected ITestOutputHelper Output { get; }
        protected TestProfiler Profiler { get; }
    }
}