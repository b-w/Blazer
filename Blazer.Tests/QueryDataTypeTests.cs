namespace Blazer.Tests
{
    using System;
    using Blazer;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Testing the various datatypes a select query can return.
    /// Currently runs against the AdventureWorks 2014 database:
    /// https://msftdbprodsamples.codeplex.com/releases/view/125550
    /// 
    /// Note: the AdventureWorks schema does not contain columns for each
    /// possible datatype.
    /// 
    /// TODO: for completeness we should eventually test all possible
    /// datatypes, though for now it'll suffice to assume that if we can deal
    /// with nullable ints, we'll be fine with nullable shorts as well, etc.
    /// </summary>
    [TestClass]
    public class QueryDataTypeTests
    {
        [TestMethod]
        public void Query_Select_String()
        {
            // arrange
            AWProduct product;

            // act
            using (var conn = TestResources.GetAdventureWorksConnection())
            {
                conn.Open();
                product = conn.First<AWProduct>("SELECT * FROM [Production].[Product] WHERE [ProductID] = @Id", new { Id = 328 });
                conn.Close();
            }

            // assert
            Assert.AreEqual("Mountain End Caps", product.Name);
        }

        [TestMethod]
        public void Query_Select_String_Nullable()
        {
            // arrange
            AWProduct product1, product2;

            // act
            using (var conn = TestResources.GetAdventureWorksConnection())
            {
                conn.Open();
                product1 = conn.First<AWProduct>("SELECT * FROM [Production].[Product] WHERE [ProductID] = @Id", new { Id = 328 });
                product2 = conn.First<AWProduct>("SELECT * FROM [Production].[Product] WHERE [ProductID] = @Id", new { Id = 332 });
                conn.Close();
            }

            // assert
            Assert.IsNull(product1.Color);
            Assert.AreEqual("Silver", product2.Color);
        }

        [TestMethod]
        public void Query_Select_Boolean()
        {
            // arrange
            AWProduct product1, product2;

            // act
            using (var conn = TestResources.GetAdventureWorksConnection())
            {
                conn.Open();
                product1 = conn.First<AWProduct>("SELECT * FROM [Production].[Product] WHERE [ProductID] = @Id", new { Id = 328 });
                product2 = conn.First<AWProduct>("SELECT * FROM [Production].[Product] WHERE [ProductID] = @Id", new { Id = 326 });
                conn.Close();
            }

            // assert
            Assert.IsTrue(product1.MakeFlag);
            Assert.IsFalse(product2.MakeFlag);
        }

        [TestMethod]
        public void Query_Select_Byte()
        {
            // arrange
            AWDocument document;

            // act
            using (var conn = TestResources.GetAdventureWorksConnection())
            {
                conn.Open();
                document = conn.First<AWDocument>("SELECT * FROM [Production].[Document] WHERE [Title] = @Title", new { Title = "Repair and Service Guidelines" });
                conn.Close();
            }

            // assert
            Assert.AreEqual(2, document.Status);
        }

        [TestMethod]
        public void Query_Select_Short()
        {
            // arrange
            AWProduct product;

            // act
            using (var conn = TestResources.GetAdventureWorksConnection())
            {
                conn.Open();
                product = conn.First<AWProduct>("SELECT * FROM [Production].[Product] WHERE [ProductID] = @Id", new { Id = 328 });
                conn.Close();
            }

            // assert
            Assert.AreEqual(750, product.ReorderPoint);
        }

        [TestMethod]
        public void Query_Select_Int()
        {
            // arrange
            AWProduct product;

            // act
            using (var conn = TestResources.GetAdventureWorksConnection())
            {
                conn.Open();
                product = conn.First<AWProduct>("SELECT * FROM [Production].[Product] WHERE [ProductID] = @Id", new { Id = 328 });
                conn.Close();
            }

            // assert
            Assert.AreEqual(1, product.DaysToManufacture);
        }

        [TestMethod]
        public void Query_Select_Int_Nullable()
        {
            // arrange
            AWProduct product1, product2;

            // act
            using (var conn = TestResources.GetAdventureWorksConnection())
            {
                conn.Open();
                product1 = conn.First<AWProduct>("SELECT * FROM [Production].[Product] WHERE [ProductID] = @Id", new { Id = 328 });
                product2 = conn.First<AWProduct>("SELECT * FROM [Production].[Product] WHERE [ProductID] = @Id", new { Id = 680 });
                conn.Close();
            }

            // assert
            Assert.IsNull(product1.ProductModelID);
            Assert.AreEqual(6, product2.ProductModelID);
        }

        [TestMethod]
        public void Query_Select_Decimal()
        {
            // arrange
            AWProduct product;

            // act
            using (var conn = TestResources.GetAdventureWorksConnection())
            {
                conn.Open();
                product = conn.First<AWProduct>("SELECT * FROM [Production].[Product] WHERE [ProductID] = @Id", new { Id = 680 });
                conn.Close();
            }

            // assert
            Assert.AreEqual(1431.50M, product.ListPrice);
        }

        [TestMethod]
        public void Query_Select_Decimal_Nullable()
        {
            // arrange
            AWSalesPerson salesPerson1, salesPerson2;

            // act
            using (var conn = TestResources.GetAdventureWorksConnection())
            {
                conn.Open();
                salesPerson1 = conn.First<AWSalesPerson>("SELECT * FROM [Sales].[SalesPerson] WHERE [BusinessEntityID] = @Id", new { Id = 274 });
                salesPerson2 = conn.First<AWSalesPerson>("SELECT * FROM [Sales].[SalesPerson] WHERE [BusinessEntityID] = @Id", new { Id = 275 });
                conn.Close();
            }

            // assert
            Assert.IsNull(salesPerson1.SalesQuota);
            Assert.AreEqual(300000M, salesPerson2.SalesQuota);
        }

        [TestMethod]
        public void Query_Select_Guid()
        {
            // arrange
            AWProduct product;

            // act
            using (var conn = TestResources.GetAdventureWorksConnection())
            {
                conn.Open();
                product = conn.First<AWProduct>("SELECT * FROM [Production].[Product] WHERE [ProductID] = @Id", new { Id = 328 });
                conn.Close();
            }

            // assert
            Assert.AreEqual(new Guid("6070B1EA-59B7-4F8B-950F-2BE07D00449D"), product.rowguid);
        }

        [TestMethod]
        public void Query_Select_DateTime()
        {
            // arrange
            AWProduct product;

            // act
            using (var conn = TestResources.GetAdventureWorksConnection())
            {
                conn.Open();
                product = conn.First<AWProduct>("SELECT * FROM [Production].[Product] WHERE [ProductID] = @Id", new { Id = 328 });
                conn.Close();
            }

            // assert
            Assert.AreEqual(new DateTime(2008, 4, 30), product.SellStartDate);
        }

        [TestMethod]
        public void Query_Select_DateTime_Nullable()
        {
            // arrange
            AWProduct product1, product2;

            // act
            using (var conn = TestResources.GetAdventureWorksConnection())
            {
                conn.Open();
                product1 = conn.First<AWProduct>("SELECT * FROM [Production].[Product] WHERE [ProductID] = @Id", new { Id = 328 });
                product2 = conn.First<AWProduct>("SELECT * FROM [Production].[Product] WHERE [ProductID] = @Id", new { Id = 709 });
                conn.Close();
            }

            // assert
            Assert.IsNull(product1.SellEndDate);
            Assert.AreEqual(new DateTime(2012, 5, 29), product2.SellEndDate);
        }
    }
}
