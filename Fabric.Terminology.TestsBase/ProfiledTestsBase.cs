using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using Fabric.Terminology.Domain;
using Xunit.Abstractions;

namespace Fabric.Terminology.TestsBase
{
    public abstract class ProfiledTestsBase : TestsBase
    {       
        protected ProfiledTestsBase(ITestOutputHelper output, ConfigTestFor testType = ConfigTestFor.Unit) 
            : base(output, testType)
        {
            Stopwatch = new Stopwatch();
        }

        protected virtual Stopwatch Stopwatch { get; }

        protected T ExecuteTimed<T>(Func<T> toWatch, string msg = "")
        {
            Stopwatch.Reset();
            Stopwatch.Start();
            var result = toWatch.Invoke();
            Stopwatch.Stop();
            OutputTimer(msg);
            return result;

        }

        protected T ExecuteTimedAysnc<T>(Func<Task<T>> toWatch, string msg = "")
        {
            Stopwatch.Reset();
            Stopwatch.Start();
            var result = toWatch.Invoke().Result;
            Stopwatch.Stop();
            OutputTimer(msg);

            return result;
        }

        protected void InvokeTimed(Action toWatch, string msg = "")
        {
            Stopwatch.Reset();
            Stopwatch.Start();
            toWatch.Invoke();
            Stopwatch.Stop();
            OutputTimer(msg);
        }

        private void OutputTimer(string msg = "")
        {
            if (!msg.IsNullOrWhiteSpace()) msg += Environment.NewLine;
            Output.WriteLine($"{msg}Operation completed in {Stopwatch.Elapsed.TotalSeconds} seconds.");
        }
    }
}
