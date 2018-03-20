namespace Fabric.Terminology.TestsBase
{
    using Xunit.Abstractions;

    public abstract class OutputTestBase
    {
        protected OutputTestBase(ITestOutputHelper output)
        {
            this.Output = output;
            this.Profiler = new TestProfiler(output);
        }

        protected ITestOutputHelper Output { get; }

        protected TestProfiler Profiler { get; }
    }
}