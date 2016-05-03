namespace Blazer.PerformanceTests
{
    using System;
    using System.Collections.Generic;
    using System.IO;
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

            Console.WriteLine("Warming up DB...");
            TestHandCrafted(Console.Out);
            Console.WriteLine("DB warmup completed!");
            Console.WriteLine();

            using (var sw = new StreamWriter("results.csv"))
            {
                TestHandCrafted(sw);
                TestBlazer(sw);
                TestL2S(sw);
                TestL2SCC(sw);
                TestEF(sw);
                TestEFCC(sw);
                TestDapper(sw);
                TestOrmLite(sw);
                TestPetaPoco(sw);
            }

            Console.WriteLine();
            Console.WriteLine("Performance tests completed!");

            //Console.ReadKey();
        }

        static void RunTest(Func<TestBase> testFactory, TextWriter resultStream)
        {
            Console.Write("Starting test set...");
            var multiTester = new MultiTestRunner(testFactory, 10);
            var results = multiTester.Run();
            resultStream.WriteLine("{0}\t{1}\t{2:0.00}\t{3:0.00}", results.Name, results.SampleCount, results.DurationAvg, results.DurationStdDv);
            Console.WriteLine("DONE!");
        }

        static void TestHandCrafted(TextWriter resultStream)
        {
            Console.WriteLine("Testing: Hand-Crafted ADO.NET");
            RunTest(() => new HCSmallSelectTest(), resultStream);
            RunTest(() => new HCLargeSelectTest(), resultStream);
            RunTest(() => new HCHugeSelectTest(), resultStream);
            RunTest(() => new HCSingleSelectManyTimesTest(500), resultStream);
            RunTest(() => new HCSingleSelectManyTimesTest(5000), resultStream);
            RunTest(() => new HCSingleSelectManyTimesTest(50000), resultStream);
        }

        static void TestBlazer(TextWriter resultStream)
        {
            Console.WriteLine("Testing: Blazer");
            RunTest(() => new BlazerSmallSelectTest(), resultStream);
            RunTest(() => new BlazerLargeSelectTest(), resultStream);
            RunTest(() => new BlazerHugeSelectTest(), resultStream);
            RunTest(() => new BlazerSingleSelectManyTimesTest(500), resultStream);
            RunTest(() => new BlazerSingleSelectManyTimesTest(5000), resultStream);
            RunTest(() => new BlazerSingleSelectManyTimesTest(50000), resultStream);
            RunTest(() => new BlazerSingleDynamicSelectManyTimesTest(500), resultStream);
            RunTest(() => new BlazerSingleDynamicSelectManyTimesTest(5000), resultStream);
            RunTest(() => new BlazerSingleDynamicSelectManyTimesTest(50000), resultStream);
        }

        static void TestL2S(TextWriter resultStream)
        {
            Console.WriteLine("Testing: Linq2SQL");
            RunTest(() => new L2SSmallSelectTest(), resultStream);
            RunTest(() => new L2SLargeSelectTest(), resultStream);
            RunTest(() => new L2SHugeSelectTest(), resultStream);
            RunTest(() => new L2SSingleSelectManyTimesTest(500), resultStream);
            RunTest(() => new L2SSingleSelectManyTimesTest(5000), resultStream);
            RunTest(() => new L2SSingleSelectManyTimesTest(50000), resultStream);
        }

        static void TestL2SCC(TextWriter resultStream)
        {
            Console.WriteLine("Testing: Linq2SQL (change tracking enabled)");
            RunTest(() => new L2SCCSmallSelectTest(), resultStream);
            RunTest(() => new L2SCCLargeSelectTest(), resultStream);
            RunTest(() => new L2SCCHugeSelectTest(), resultStream);
            RunTest(() => new L2SCCSingleSelectManyTimesTest(500), resultStream);
            RunTest(() => new L2SCCSingleSelectManyTimesTest(5000), resultStream);
            RunTest(() => new L2SCCSingleSelectManyTimesTest(50000), resultStream);
        }

        static void TestEF(TextWriter resultStream)
        {
            Console.WriteLine("Testing: Entity Framework 6");
            RunTest(() => new EFSmallSelectTest(), resultStream);
            RunTest(() => new EFLargeSelectTest(), resultStream);
            RunTest(() => new EFHugeSelectTest(), resultStream);
            RunTest(() => new EFSingleSelectManyTimesTest(500), resultStream);
            RunTest(() => new EFSingleSelectManyTimesTest(5000), resultStream);
            RunTest(() => new EFSingleSelectManyTimesTest(50000), resultStream);
        }

        static void TestEFCC(TextWriter resultStream)
        {
            Console.WriteLine("Testing: Entity Framework 6 (change tracking enabled)");
            RunTest(() => new EFCCSmallSelectTest(), resultStream);
            RunTest(() => new EFCCLargeSelectTest(), resultStream);
            RunTest(() => new EFCCHugeSelectTest(), resultStream);
            RunTest(() => new EFCCSingleSelectManyTimesTest(500), resultStream);
            RunTest(() => new EFCCSingleSelectManyTimesTest(5000), resultStream);
            RunTest(() => new EFCCSingleSelectManyTimesTest(50000), resultStream);
        }

        static void TestDapper(TextWriter resultStream)
        {
            Console.WriteLine("Testing: Dapper v1.42.0");
            RunTest(() => new DapperSmallSelectTest(), resultStream);
            RunTest(() => new DapperLargeSelectTest(), resultStream);
            RunTest(() => new DapperHugeSelectTest(), resultStream);
            RunTest(() => new DapperSingleSelectManyTimesTest(500), resultStream);
            RunTest(() => new DapperSingleSelectManyTimesTest(5000), resultStream);
            RunTest(() => new DapperSingleSelectManyTimesTest(50000), resultStream);
            RunTest(() => new DapperSingleDynamicSelectManyTimesTest(500), resultStream);
            RunTest(() => new DapperSingleDynamicSelectManyTimesTest(5000), resultStream);
            RunTest(() => new DapperSingleDynamicSelectManyTimesTest(50000), resultStream);
        }

        static void TestOrmLite(TextWriter resultStream)
        {
            Console.WriteLine("Testing: OrmLite v4.0.56");
            RunTest(() => new OrmLiteSmallSelectTest(), resultStream);
            RunTest(() => new OrmLiteLargeSelectTest(), resultStream);
            RunTest(() => new OrmLiteHugeSelectTest(), resultStream);
            RunTest(() => new OrmLiteSingleSelectManyTimesTest(500), resultStream);
            RunTest(() => new OrmLiteSingleSelectManyTimesTest(5000), resultStream);
            RunTest(() => new OrmLiteSingleSelectManyTimesTest(50000), resultStream);
        }

        static void TestPetaPoco(TextWriter resultStream)
        {
            Console.WriteLine("Testing: PetaPoco v5.1.1.171");
            RunTest(() => new PetaPocoSmallSelectTest(), resultStream);
            RunTest(() => new PetaPocoLargeSelectTest(), resultStream);
            RunTest(() => new PetaPocoHugeSelectTest(), resultStream);
            RunTest(() => new PetaPocoSingleSelectManyTimesTest(500), resultStream);
            RunTest(() => new PetaPocoSingleSelectManyTimesTest(5000), resultStream);
            RunTest(() => new PetaPocoSingleSelectManyTimesTest(50000), resultStream);
            RunTest(() => new PetaPocoSingleDynamicSelectManyTimesTest(500), resultStream);
            RunTest(() => new PetaPocoSingleDynamicSelectManyTimesTest(5000), resultStream);
            RunTest(() => new PetaPocoSingleDynamicSelectManyTimesTest(50000), resultStream);
        }
    }
}
