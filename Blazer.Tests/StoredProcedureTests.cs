namespace Blazer.Tests
{
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using Blazer;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class StoredProcedureTests
    {
        [TestMethod]
        public void Sp_Is_Normal_Query()
        {
            // arrange
            IList<AWEmployeeManager> emplManagers;

            // act
            using (var conn = TestResources.GetAdventureWorksConnection())
            {
                conn.Open();
                emplManagers = conn.Query<AWEmployeeManager>(
                    "uspGetEmployeeManagers", new { BusinessEntityID = 34 },
                    config: new CommandConfiguration() { CommandType = CommandType.StoredProcedure })
                    .ToList();
                conn.Close();
            }

            // assert
            Assert.AreEqual(3, emplManagers.Count);
            for (int i = 0; i < emplManagers.Count; i++)
            {
                Assert.IsNotNull(emplManagers[i]);
            }

            Assert.AreEqual(34, emplManagers[0].BusinessEntityID);
            Assert.AreEqual("Suchitra", emplManagers[0].FirstName);
            Assert.AreEqual("Jo", emplManagers[0].ManagerFirstName);

            Assert.AreEqual(27, emplManagers[1].BusinessEntityID);
            Assert.AreEqual("Jo", emplManagers[1].FirstName);
            Assert.AreEqual("Peter", emplManagers[1].ManagerFirstName);

            Assert.AreEqual(26, emplManagers[2].BusinessEntityID);
            Assert.AreEqual("Peter", emplManagers[2].FirstName);
            Assert.AreEqual("James", emplManagers[2].ManagerFirstName);
        }

        [TestMethod]
        public void Sp_InputOutput_Parameters()
        {
            // arrange
            var spParams = new SpParameters();
            spParams.AddInput("@x", 40);
            spParams.AddInput("@y", 2);
            spParams.AddInputOutput("@i", 8);
            spParams.AddOutput("@msg", DbType.String, size: 20);
            spParams.SetReturn("@return", DbType.Int32);

            // act
            using (var conn = TestResources.GetAdventureWorksConnection())
            {
                conn.Open();
                conn.StoredProcedure("SpBlazerTest", spParams);
                conn.Close();
            }

            // assert
            Assert.AreEqual(42, spParams.GetReturnValue<int>());
            Assert.AreEqual(9, spParams.GetOutputValue<int>("@i"));
            Assert.AreEqual("Hello Blazer!", spParams.GetOutputValue<string>("@msg"));
        }
    }
}
