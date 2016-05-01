namespace Blazer.PerformanceTests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Tests;

    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Starting performance tests...");
            Console.WriteLine();

            TestBlazer();
            TestEF();

            Console.WriteLine();
            Console.WriteLine("Performance tests completed!");

            //Console.ReadKey();
        }

        static void RunTest(TestBase test)
        {
            Console.Write(test.Name.PadRight(60, ' '));
            var results = test.Run();
            Console.WriteLine($"{results.Duration}ms".PadLeft(10, ' '));
        }

        static void TestBlazer()
        {
            using (var test = new BlazerSmallSelectTest())
            {
                RunTest(test);
            }
            using (var test = new BlazerLargeSelectTest())
            {
                RunTest(test);
            }
            using (var test = new BlazerHugeSelectTest())
            {
                RunTest(test);
            }
            using (var test = new BlazerSingleSelectManyTimesTest())
            {
                RunTest(test);
            }
        }

        static void TestEF()
        {
            using (var test = new EFSmallSelectTest())
            {
                RunTest(test);
            }
            using (var test = new EFLargeSelectTest())
            {
                RunTest(test);
            }
            using (var test = new EFHugeSelectTest())
            {
                RunTest(test);
            }
            using (var test = new EFSingleSelectManyTimesTest())
            {
                RunTest(test);
            }
        }
    }
}
