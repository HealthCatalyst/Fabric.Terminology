﻿using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Fabric.Terminology.Domain;
using Xunit.Abstractions;

namespace Fabric.Terminology.TestsBase
{
    public class TestProfiler
    {
        public TestProfiler(ITestOutputHelper output)
        {
            Output = output;
        }

        protected ITestOutputHelper Output { get; }

        public T ExecuteTimed<T>(Func<T> toWatch, string msg = "")
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            var result = toWatch.Invoke();
            stopwatch.Stop();
            OutputTimer(stopwatch, msg);
            return result;
        }

        public T ExecuteTimed<T>(Func<Task<T>> toWatch, string msg = "")
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            var result = toWatch.Invoke().Result;
            stopwatch.Stop();
            OutputTimer(stopwatch, msg);

            return result;
        }

        public void ExecuteTimed(Action toWatch, string msg = "")
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            toWatch.Invoke();
            stopwatch.Stop();
            OutputTimer(stopwatch, msg);
        }

        private void OutputTimer(Stopwatch stopwatch, string msg = "")
        {
            if (!msg.IsNullOrWhiteSpace()) msg += Environment.NewLine;
            Output.WriteLine($"{msg}Operation completed in {stopwatch.Elapsed.TotalSeconds} seconds.");
        }
    }
}