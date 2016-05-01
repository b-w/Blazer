namespace Blazer.PerformanceTests.Tests
{
    public class TestResult
    {
        public TestResult(string name, long duration)
        {
            Name = name;
            Duration = duration;
        }

        public string Name { get; }

        public long Duration { get; }
    }
}
