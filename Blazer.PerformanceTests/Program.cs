namespace Blazer.PerformanceTests
{
    using System;
    using System.IO;
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

            using (var sw = new StreamWriter("results.csv", false))
            {
                //TestHandCrafted(sw);
                TestBlazer(sw);
                TestBlazerFormattableString(sw);
                //TestL2S(sw);
                //TestL2SCC(sw);
                //TestEF(sw);
                //TestEFCC(sw);
                //TestDapper(sw);
                //TestOrmLite(sw);
                //TestPetaPoco(sw);
                //TestSimpleData(sw);
                //TestMassive(sw);
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
            RunTest(() => new HCSelectTest(500), resultStream);
            RunTest(() => new HCSelectTest(5000), resultStream);
            RunTest(() => new HCSelectTest(50000), resultStream);
            RunTest(() => new HCSingleSelectManyTimesTest(500), resultStream);
            RunTest(() => new HCSingleSelectManyTimesTest(5000), resultStream);
            RunTest(() => new HCSingleSelectManyTimesTest(50000), resultStream);
        }

        static void TestBlazer(TextWriter resultStream)
        {
            Console.WriteLine("Testing: Blazer");
            //RunTest(() => new BlazerSelectTest(500), resultStream);
            //RunTest(() => new BlazerSelectTest(5000), resultStream);
            //RunTest(() => new BlazerSelectTest(50000), resultStream);
            //RunTest(() => new BlazerDynamicSelectTest(500), resultStream);
            //RunTest(() => new BlazerDynamicSelectTest(5000), resultStream);
            //RunTest(() => new BlazerDynamicSelectTest(50000), resultStream);
            RunTest(() => new BlazerSingleSelectManyTimesTest(500), resultStream);
            RunTest(() => new BlazerSingleSelectManyTimesTest(5000), resultStream);
            //RunTest(() => new BlazerSingleSelectManyTimesTest(50000), resultStream);
            //RunTest(() => new BlazerSingleDynamicSelectManyTimesTest(500), resultStream);
            //RunTest(() => new BlazerSingleDynamicSelectManyTimesTest(5000), resultStream);
            //RunTest(() => new BlazerSingleDynamicSelectManyTimesTest(50000), resultStream);
        }

        static void TestBlazerFormattableString(TextWriter resultStream)
        {
            Console.WriteLine("Testing: Blazer (opt FormattableString)");
            RunTest(() => new BlazerSingleSelectManyTimesTestF(500), resultStream);
            RunTest(() => new BlazerSingleSelectManyTimesTestF(5000), resultStream);
        }

        static void TestL2S(TextWriter resultStream)
        {
            Console.WriteLine("Testing: Linq2SQL");
            RunTest(() => new L2SSelectTest(500), resultStream);
            RunTest(() => new L2SSelectTest(5000), resultStream);
            RunTest(() => new L2SSelectTest(50000), resultStream);
            RunTest(() => new L2SSingleSelectManyTimesTest(500), resultStream);
            RunTest(() => new L2SSingleSelectManyTimesTest(5000), resultStream);
            RunTest(() => new L2SSingleSelectManyTimesTest(50000), resultStream);
        }

        static void TestL2SCC(TextWriter resultStream)
        {
            Console.WriteLine("Testing: Linq2SQL (change tracking enabled)");
            RunTest(() => new L2SCCSelectTest(500), resultStream);
            RunTest(() => new L2SCCSelectTest(5000), resultStream);
            RunTest(() => new L2SCCSelectTest(50000), resultStream);
            RunTest(() => new L2SCCSingleSelectManyTimesTest(500), resultStream);
            RunTest(() => new L2SCCSingleSelectManyTimesTest(5000), resultStream);
            RunTest(() => new L2SCCSingleSelectManyTimesTest(50000), resultStream);
        }

        static void TestEF(TextWriter resultStream)
        {
            Console.WriteLine("Testing: Entity Framework 6");
            RunTest(() => new EFSelectTest(500), resultStream);
            RunTest(() => new EFSelectTest(5000), resultStream);
            RunTest(() => new EFSelectTest(50000), resultStream);
            RunTest(() => new EFSingleSelectManyTimesTest(500), resultStream);
            RunTest(() => new EFSingleSelectManyTimesTest(5000), resultStream);
            RunTest(() => new EFSingleSelectManyTimesTest(50000), resultStream);
        }

        static void TestEFCC(TextWriter resultStream)
        {
            Console.WriteLine("Testing: Entity Framework 6 (change tracking enabled)");
            RunTest(() => new EFCCSelectTest(500), resultStream);
            RunTest(() => new EFCCSelectTest(5000), resultStream);
            RunTest(() => new EFCCSelectTest(50000), resultStream);
            RunTest(() => new EFCCSingleSelectManyTimesTest(500), resultStream);
            RunTest(() => new EFCCSingleSelectManyTimesTest(5000), resultStream);
            RunTest(() => new EFCCSingleSelectManyTimesTest(50000), resultStream);
        }

        static void TestDapper(TextWriter resultStream)
        {
            Console.WriteLine("Testing: Dapper v1.42.0");
            //RunTest(() => new DapperSelectTest(500), resultStream);
            //RunTest(() => new DapperSelectTest(5000), resultStream);
            //RunTest(() => new DapperSelectTest(50000), resultStream);
            RunTest(() => new DapperDynamicSelectTest(500), resultStream);
            RunTest(() => new DapperDynamicSelectTest(5000), resultStream);
            RunTest(() => new DapperDynamicSelectTest(50000), resultStream);
            //RunTest(() => new DapperSingleSelectManyTimesTest(500), resultStream);
            //RunTest(() => new DapperSingleSelectManyTimesTest(5000), resultStream);
            //RunTest(() => new DapperSingleSelectManyTimesTest(50000), resultStream);
            //RunTest(() => new DapperSingleDynamicSelectManyTimesTest(500), resultStream);
            //RunTest(() => new DapperSingleDynamicSelectManyTimesTest(5000), resultStream);
            //RunTest(() => new DapperSingleDynamicSelectManyTimesTest(50000), resultStream);
        }

        static void TestOrmLite(TextWriter resultStream)
        {
            Console.WriteLine("Testing: OrmLite v4.0.56");
            RunTest(() => new OrmLiteSelectTest(500), resultStream);
            RunTest(() => new OrmLiteSelectTest(5000), resultStream);
            RunTest(() => new OrmLiteSelectTest(50000), resultStream);
            RunTest(() => new OrmLiteSingleSelectManyTimesTest(500), resultStream);
            RunTest(() => new OrmLiteSingleSelectManyTimesTest(5000), resultStream);
            RunTest(() => new OrmLiteSingleSelectManyTimesTest(50000), resultStream);
        }

        static void TestPetaPoco(TextWriter resultStream)
        {
            Console.WriteLine("Testing: PetaPoco v5.1.1.171");
            //RunTest(() => new PetaPocoSelectTest(500), resultStream);
            //RunTest(() => new PetaPocoSelectTest(5000), resultStream);
            //RunTest(() => new PetaPocoSelectTest(50000), resultStream);
            RunTest(() => new PetaPocoDynamicSelectTest(500), resultStream);
            RunTest(() => new PetaPocoDynamicSelectTest(5000), resultStream);
            RunTest(() => new PetaPocoDynamicSelectTest(50000), resultStream);
            //RunTest(() => new PetaPocoSingleSelectManyTimesTest(500), resultStream);
            //RunTest(() => new PetaPocoSingleSelectManyTimesTest(5000), resultStream);
            //RunTest(() => new PetaPocoSingleSelectManyTimesTest(50000), resultStream);
            //RunTest(() => new PetaPocoSingleDynamicSelectManyTimesTest(500), resultStream);
            //RunTest(() => new PetaPocoSingleDynamicSelectManyTimesTest(5000), resultStream);
            //RunTest(() => new PetaPocoSingleDynamicSelectManyTimesTest(50000), resultStream);
        }

        static void TestSimpleData(TextWriter resultStream)
        {
            Console.WriteLine("Testing: Simple.Data v0.19.0");
            RunTest(() => new SimpleDataDynamicSelectTest(500), resultStream);
            RunTest(() => new SimpleDataDynamicSelectTest(5000), resultStream);
            RunTest(() => new SimpleDataDynamicSelectTest(50000), resultStream);
            //RunTest(() => new SimpleDataSingleDynamicSelectManyTimesTest(500), resultStream);
            //RunTest(() => new SimpleDataSingleDynamicSelectManyTimesTest(5000), resultStream);
            //RunTest(() => new SimpleDataSingleDynamicSelectManyTimesTest(50000), resultStream);
        }

        static void TestMassive(TextWriter resultStream)
        {
            Console.WriteLine("Testing: Massive v2.0");
            RunTest(() => new MassiveDynamicSelectTest(500), resultStream);
            RunTest(() => new MassiveDynamicSelectTest(5000), resultStream);
            RunTest(() => new MassiveDynamicSelectTest(50000), resultStream);
            //RunTest(() => new MassiveSingleDynamicSelectManyTimesTest(500), resultStream);
            //RunTest(() => new MassiveSingleDynamicSelectManyTimesTest(5000), resultStream);
            //RunTest(() => new MassiveSingleDynamicSelectManyTimesTest(50000), resultStream);
        }
    }
}
