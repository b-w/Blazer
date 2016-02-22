namespace Blazer.Tests
{
    using System;
    using System.Data.SqlClient;
    using Blazer;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class QueryTests
    {
        [TestMethod]
        public void Basic_Query()
        {
            // arrange
            AWProduct product;

            // act
            using (var conn = new SqlConnection(@"Server=.\LOCALSQL;Database=AdventureWorks;Integrated Security=True"))
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
    }
}
