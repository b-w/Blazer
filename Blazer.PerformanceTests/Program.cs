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

            TestHandCrafted();
            Console.WriteLine();
            TestBlazer();
            Console.WriteLine();
            TestL2S();
            Console.WriteLine();
            TestL2SCC();
            Console.WriteLine();
            TestEF();
            Console.WriteLine();
            TestEFCC();
            Console.WriteLine();
            TestDapper();
            Console.WriteLine();
            TestOrmLite();
            Console.WriteLine();
            TestPetaPoco();

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

        static void TestHandCrafted()
        {
            using (var test = new HCSmallSelectTest())
            {
                RunTest(test);
            }
            using (var test = new HCLargeSelectTest())
            {
                RunTest(test);
            }
            using (var test = new HCHugeSelectTest())
            {
                RunTest(test);
            }
            using (var test = new HCSingleSelectManyTimesTest(500))
            {
                RunTest(test);
            }
            using (var test = new HCSingleSelectManyTimesTest(5000))
            {
                RunTest(test);
            }
            using (var test = new HCSingleSelectManyTimesTest(50000))
            {
                RunTest(test);
            }
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
            using (var test = new BlazerSingleSelectManyTimesTest(500))
            {
                RunTest(test);
            }
            using (var test = new BlazerSingleSelectManyTimesTest(5000))
            {
                RunTest(test);
            }
            using (var test = new BlazerSingleSelectManyTimesTest(50000))
            {
                RunTest(test);
            }
            using (var test = new BlazerSingleDynamicSelectManyTimesTest(500))
            {
                RunTest(test);
            }
            using (var test = new BlazerSingleDynamicSelectManyTimesTest(5000))
            {
                RunTest(test);
            }
            using (var test = new BlazerSingleDynamicSelectManyTimesTest(50000))
            {
                RunTest(test);
            }
        }

        static void TestL2S()
        {
            using (var test = new L2SSmallSelectTest())
            {
                RunTest(test);
            }
            using (var test = new L2SLargeSelectTest())
            {
                RunTest(test);
            }
            using (var test = new L2SHugeSelectTest())
            {
                RunTest(test);
            }
            using (var test = new L2SSingleSelectManyTimesTest(500))
            {
                RunTest(test);
            }
            using (var test = new L2SSingleSelectManyTimesTest(5000))
            {
                RunTest(test);
            }
            using (var test = new L2SSingleSelectManyTimesTest(50000))
            {
                RunTest(test);
            }
        }

        static void TestL2SCC()
        {
            using (var test = new L2SCCSmallSelectTest())
            {
                RunTest(test);
            }
            using (var test = new L2SCCLargeSelectTest())
            {
                RunTest(test);
            }
            using (var test = new L2SCCHugeSelectTest())
            {
                RunTest(test);
            }
            using (var test = new L2SCCSingleSelectManyTimesTest(500))
            {
                RunTest(test);
            }
            using (var test = new L2SCCSingleSelectManyTimesTest(5000))
            {
                RunTest(test);
            }
            using (var test = new L2SCCSingleSelectManyTimesTest(50000))
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
            using (var test = new EFSingleSelectManyTimesTest(500))
            {
                RunTest(test);
            }
            using (var test = new EFSingleSelectManyTimesTest(5000))
            {
                RunTest(test);
            }
            using (var test = new EFSingleSelectManyTimesTest(50000))
            {
                RunTest(test);
            }
        }

        static void TestEFCC()
        {
            using (var test = new EFCCSmallSelectTest())
            {
                RunTest(test);
            }
            using (var test = new EFCCLargeSelectTest())
            {
                RunTest(test);
            }
            using (var test = new EFCCHugeSelectTest())
            {
                RunTest(test);
            }
            using (var test = new EFCCSingleSelectManyTimesTest(500))
            {
                RunTest(test);
            }
            using (var test = new EFCCSingleSelectManyTimesTest(5000))
            {
                RunTest(test);
            }
            using (var test = new EFCCSingleSelectManyTimesTest(50000))
            {
                RunTest(test);
            }
        }

        static void TestDapper()
        {
            using (var test = new DapperSmallSelectTest())
            {
                RunTest(test);
            }
            using (var test = new DapperLargeSelectTest())
            {
                RunTest(test);
            }
            using (var test = new DapperHugeSelectTest())
            {
                RunTest(test);
            }
            using (var test = new DapperSingleSelectManyTimesTest(500))
            {
                RunTest(test);
            }
            using (var test = new DapperSingleSelectManyTimesTest(5000))
            {
                RunTest(test);
            }
            using (var test = new DapperSingleSelectManyTimesTest(50000))
            {
                RunTest(test);
            }
            using (var test = new DapperSingleDynamicSelectManyTimesTest(500))
            {
                RunTest(test);
            }
            using (var test = new DapperSingleDynamicSelectManyTimesTest(5000))
            {
                RunTest(test);
            }
            using (var test = new DapperSingleDynamicSelectManyTimesTest(50000))
            {
                RunTest(test);
            }
        }

        static void TestOrmLite()
        {
            using (var test = new OrmLiteSmallSelectTest())
            {
                RunTest(test);
            }
            using (var test = new OrmLiteLargeSelectTest())
            {
                RunTest(test);
            }
            using (var test = new OrmLiteHugeSelectTest())
            {
                RunTest(test);
            }
            using (var test = new OrmLiteSingleSelectManyTimesTest(500))
            {
                RunTest(test);
            }
            using (var test = new OrmLiteSingleSelectManyTimesTest(5000))
            {
                RunTest(test);
            }
            using (var test = new OrmLiteSingleSelectManyTimesTest(50000))
            {
                RunTest(test);
            }
        }

        static void TestPetaPoco()
        {
            using (var test = new PetaPocoSmallSelectTest())
            {
                RunTest(test);
            }
            using (var test = new PetaPocoLargeSelectTest())
            {
                RunTest(test);
            }
            using (var test = new PetaPocoHugeSelectTest())
            {
                RunTest(test);
            }
            using (var test = new PetaPocoSingleSelectManyTimesTest(500))
            {
                RunTest(test);
            }
            using (var test = new PetaPocoSingleSelectManyTimesTest(5000))
            {
                RunTest(test);
            }
            using (var test = new PetaPocoSingleSelectManyTimesTest(50000))
            {
                RunTest(test);
            }
            using (var test = new PetaPocoSingleDynamicSelectManyTimesTest(500))
            {
                RunTest(test);
            }
            using (var test = new PetaPocoSingleDynamicSelectManyTimesTest(5000))
            {
                RunTest(test);
            }
            using (var test = new PetaPocoSingleDynamicSelectManyTimesTest(50000))
            {
                RunTest(test);
            }
        }
    }
}
