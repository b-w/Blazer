namespace Blazer.PerformanceTests.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class MultiTestResult
    {
        protected readonly IList<TestResult> m_results = new List<TestResult>();

        public MultiTestResult(string name)
        {
            Name = name;
        }

        public string Name { get; }

        public void AddResult(TestResult result)
        {
            m_results.Add(result);
        }

        public int SampleCount => m_results.Count;

        public double DurationAvg => m_results.Average(x => x.Duration);

        public double DurationStdDv
        {
            get
            {
                var avg = DurationAvg;
                var variance = m_results
                    .Select(x => Math.Pow(x.Duration - avg, 2))
                    .Average();
                return Math.Sqrt(variance);
            }
        }
    }
}
