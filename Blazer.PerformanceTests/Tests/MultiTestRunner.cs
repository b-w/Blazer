namespace Blazer.PerformanceTests.Tests
{
    using System;

    public class MultiTestRunner
    {
        protected readonly Func<TestBase> m_testFactory;
        protected readonly int m_testCount;

        public MultiTestRunner(Func<TestBase> testFactory, int testCount)
        {
            m_testFactory = testFactory;
            m_testCount = testCount;
        }

        public MultiTestResult Run()
        {
            MultiTestResult result = null;
            for (int i = 0; i < m_testCount; i++)
            {
                var singleResult = RunSingle();
                if (i == 0)
                {
                    result = new MultiTestResult(singleResult.Name);
                }
                result.AddResult(singleResult);
            }
            return result;
        }

        protected TestResult RunSingle()
        {
            TestResult result;
            using (var test = m_testFactory())
            {
                result = test.Run();
            }
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();
            return result;
        }
    }
}
