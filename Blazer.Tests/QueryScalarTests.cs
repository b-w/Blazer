namespace Blazer.Tests
{
    using Blazer;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class QueryScalarTests
    {
        [TestMethod]
        public void Query_Scalar_Int()
        {
            // arrange
            int productId;

            // act
            using (var conn = TestResources.GetAdventureWorksConnection())
            {
                conn.Open();
                productId = conn.Scalar<int>("SELECT [ProductID] FROM [Production].[Product] WHERE [ProductNumber] = @Number", new { Number = "EC-M092" });
                conn.Close();
            }

            // assert
            Assert.AreEqual(328, productId);
        }

        [TestMethod]
        public void Query_Scalar_String()
        {
            // arrange
            string productName;

            // act
            using (var conn = TestResources.GetAdventureWorksConnection())
            {
                conn.Open();
                productName = conn.Scalar<string>("SELECT [Name] FROM [Production].[Product] WHERE [ProductID] = @Id", new { Id = 328 });
                conn.Close();
            }

            // assert
            Assert.AreEqual("Mountain End Caps", productName);
        }

        [TestMethod]
        public void Query_Scalar_DbNull()
        {
            // arrange
            string productName;

            // act
            using (var conn = TestResources.GetAdventureWorksConnection())
            {
                conn.Open();
                productName = conn.Scalar<string>("SELECT [Color] FROM [Production].[Product] WHERE [ProductID] = @Id", new { Id = 328 });
                conn.Close();
            }

            // assert
            Assert.IsNull(productName);
        }

        [TestMethod]
        public void Query_Scalar_NoResults()
        {
            // arrange
            string productName;

            // act
            using (var conn = TestResources.GetAdventureWorksConnection())
            {
                conn.Open();
                productName = conn.Scalar<string>("SELECT [Name] FROM [Production].[Product] WHERE [ProductID] = @Id", new { Id = 42 });
                conn.Close();
            }

            // assert
            Assert.IsNull(productName);
        }
    }
}
