namespace Blazer.PerformanceTests.Tests
{
    using System;
    using System.Diagnostics;

    public abstract class TestBase : IDisposable
    {
        readonly Stopwatch m_watch = new Stopwatch();

        public TestBase(string name)
        {
            Name = name;
        }

        public string Name { get; }

        public TestResult Run()
        {
            m_watch.Reset();
            Warmup();
            m_watch.Start();
            DoWork();
            m_watch.Stop();

            return new TestResult(Name, m_watch.ElapsedMilliseconds);
        }

        protected abstract void Warmup();

        protected abstract void DoWork();

        #region IDisposable

        protected bool m_disposed = false;

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected abstract void Dispose(bool disposing);

        #endregion
    }
}
