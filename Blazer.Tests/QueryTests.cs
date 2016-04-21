namespace Blazer.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Blazer;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class QueryTests
    {
        [TestMethod]
        public void Query_Basic()
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
            Assert.IsNotNull(product);
            Assert.AreEqual(328, product.ProductID);
            Assert.AreEqual("Mountain End Caps", product.Name);
            Assert.AreEqual(new DateTime(2008, 4, 30), product.SellStartDate);
            Assert.IsNull(product.SellEndDate);
        }

        [TestMethod]
        public void Query_Multiple_Records()
        {
            // arrange
            IList<AWProduct> products;

            // act
            using (var conn = TestResources.GetAdventureWorksConnection())
            {
                conn.Open();
                products = conn.Query<AWProduct>("SELECT * FROM [Production].[Product] WHERE [ProductID] < 100")
                    .ToList();
                conn.Close();
            }

            // assert
            Assert.AreEqual(4, products.Count);
            for (int i = 0; i < products.Count; i++)
            {
                Assert.IsNotNull(products[i]);
            }
            Assert.AreEqual(1, products[0].ProductID);
            Assert.AreEqual("Adjustable Race", products[0].Name);
            Assert.AreEqual(2, products[1].ProductID);
            Assert.AreEqual("Bearing Ball", products[1].Name);
            Assert.AreEqual(3, products[2].ProductID);
            Assert.AreEqual("BB Ball Bearing", products[2].Name);
            Assert.AreEqual(4, products[3].ProductID);
            Assert.AreEqual("Headset Ball Bearings", products[3].Name);
        }

        [TestMethod]
        public void Query_Dynamic()
        {
            // arrange
            dynamic product;

            // act
            using (var conn = TestResources.GetAdventureWorksConnection())
            {
                conn.Open();
                product = conn.Query("SELECT * FROM [Production].[Product] WHERE [ProductID] = @Id", new { Id = 328 })
                    .First();
                conn.Close();
            }

            // assert
            Assert.IsNotNull(product);
            Assert.AreEqual(328, product.ProductID);
            Assert.AreEqual("Mountain End Caps", product.Name);
            Assert.AreEqual(new DateTime(2008, 4, 30), product.SellStartDate);
            Assert.IsNull(product.SellEndDate);
        }
    }
}
