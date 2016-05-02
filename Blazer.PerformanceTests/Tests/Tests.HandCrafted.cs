namespace Blazer.PerformanceTests.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using Models.AdventureWorks;

    public class HCSmallSelectTest : TestBase
    {
        IDbConnection m_conn;

        public HCSmallSelectTest() : base("Hand-Crafted ADO.NET: SELECT 500 records")
        {
        }

        protected override void Warmup()
        {
            m_conn = TestResources.GetAdventureWorksConnection();
            m_conn.Open();
        }

        protected override void DoWork()
        {
            var products = new List<Product>();
            using (var cmd = m_conn.CreateCommand())
            {
                cmd.CommandText = "SELECT * FROM [Production].[Product] WHERE [ProductID] > 10";
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var product = new Product();

                        product.ProductID = reader.GetInt32(0);
                        product.Name = reader.GetString(1);
                        product.ProductNumber = reader.GetString(2);
                        product.MakeFlag = reader.GetBoolean(3);
                        product.FinishedGoodsFlag = reader.GetBoolean(4);
                        if (!reader.IsDBNull(5))
                        {
                            product.Color = reader.GetString(5);
                        }
                        product.SafetyStockLevel = reader.GetInt16(6);
                        product.ReorderPoint = reader.GetInt16(7);
                        product.StandardCost = reader.GetDecimal(8);
                        product.ListPrice = reader.GetDecimal(9);
                        if (!reader.IsDBNull(10))
                        {
                            product.Size = reader.GetString(10);
                        }
                        if (!reader.IsDBNull(11))
                        {
                            product.SizeUnitMeasureCode = reader.GetString(11);
                        }
                        if (!reader.IsDBNull(12))
                        {
                            product.WeightUnitMeasureCode = reader.GetString(12);
                        }
                        if (!reader.IsDBNull(13))
                        {
                            product.Weight = reader.GetDecimal(13);
                        }
                        product.DaysToManufacture = reader.GetInt32(14);
                        if (!reader.IsDBNull(15))
                        {
                            product.ProductLine = reader.GetString(15);
                        }
                        if (!reader.IsDBNull(16))
                        {
                            product.Class = reader.GetString(16);
                        }
                        if (!reader.IsDBNull(17))
                        {
                            product.Style = reader.GetString(17);
                        }
                        if (!reader.IsDBNull(18))
                        {
                            product.ProductSubcategoryID = reader.GetInt32(18);
                        }
                        if (!reader.IsDBNull(19))
                        {
                            product.ProductModelID = reader.GetInt32(19);
                        }
                        product.SellStartDate = reader.GetDateTime(20);
                        if (!reader.IsDBNull(21))
                        {
                            product.SellEndDate = reader.GetDateTime(21);
                        }
                        if (!reader.IsDBNull(22))
                        {
                            product.DiscontinuedDate = reader.GetDateTime(22);
                        }
                        product.rowguid = reader.GetGuid(23);
                        product.ModifiedDate = reader.GetDateTime(24);

                        products.Add(product);
                    }
                }
            }
            if (!products.All(x => x.ProductID > 0))
            {
                throw new ApplicationException();
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (!m_disposed)
            {
                if (disposing)
                {
                    m_conn.Close();
                    m_conn.Dispose();
                }
                m_disposed = true;
            }
        }
    }

    public class HCLargeSelectTest : TestBase
    {
        IDbConnection m_conn;

        public HCLargeSelectTest() : base("Hand-Crafted ADO.NET: SELECT 5.000 records")
        {
        }

        protected override void Warmup()
        {
            m_conn = TestResources.GetAdventureWorksConnection();
            m_conn.Open();
        }

        protected override void DoWork()
        {
            var orderDetails = new List<PurchaseOrderDetail>();
            using (var cmd = m_conn.CreateCommand())
            {
                cmd.CommandText = "SELECT TOP(5000) * FROM [Purchasing].[PurchaseOrderDetail] WHERE [PurchaseOrderDetailID] > 10";
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var detail = new PurchaseOrderDetail();

                        detail.PurchaseOrderID = reader.GetInt32(0);
                        detail.PurchaseOrderDetailID = reader.GetInt32(1);
                        detail.DueDate = reader.GetDateTime(2);
                        detail.OrderQty = reader.GetInt16(3);
                        detail.ProductID = reader.GetInt32(4);
                        detail.UnitPrice = reader.GetDecimal(5);
                        detail.LineTotal = reader.GetDecimal(6);
                        detail.ReceivedQty = reader.GetDecimal(7);
                        detail.RejectedQty = reader.GetDecimal(8);
                        detail.StockedQty = reader.GetDecimal(9);
                        detail.ModifiedDate = reader.GetDateTime(10);

                        orderDetails.Add(detail);
                    }
                }
            }
            if (!orderDetails.All(x => x.PurchaseOrderDetailID > 0))
            {
                throw new ApplicationException();
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (!m_disposed)
            {
                if (disposing)
                {
                    m_conn.Close();
                    m_conn.Dispose();
                }
                m_disposed = true;
            }
        }
    }

    public class HCHugeSelectTest : TestBase
    {
        IDbConnection m_conn;

        public HCHugeSelectTest() : base("Hand-Crafted ADO.NET: SELECT 50.000 records")
        {
        }

        protected override void Warmup()
        {
            m_conn = TestResources.GetAdventureWorksConnection();
            m_conn.Open();
        }

        protected override void DoWork()
        {
            var transactions = new List<TransactionHistory>();
            using (var cmd = m_conn.CreateCommand())
            {
                cmd.CommandText = "SELECT TOP(50000) * FROM [Production].[TransactionHistory] WHERE [TransactionID] > 100010";
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var transaction = new TransactionHistory();

                        transaction.TransactionID = reader.GetInt32(0);
                        transaction.ProductID = reader.GetInt32(1);
                        transaction.ReferenceOrderID = reader.GetInt32(2);
                        transaction.ReferenceOrderLineID = reader.GetInt32(3);
                        transaction.TransactionDate = reader.GetDateTime(4);
                        transaction.TransactionType = reader.GetString(5);
                        transaction.Quantity = reader.GetInt32(6);
                        transaction.ActualCost = reader.GetDecimal(7);
                        transaction.ModifiedDate = reader.GetDateTime(8);

                        transactions.Add(transaction);
                    }
                }
            }
            if (!transactions.All(x => x.TransactionID > 0))
            {
                throw new ApplicationException();
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (!m_disposed)
            {
                if (disposing)
                {
                    m_conn.Close();
                    m_conn.Dispose();
                }
                m_disposed = true;
            }
        }
    }

    public class HCSingleSelectManyTimesTest : TestBase
    {
        IDbConnection m_conn;
        readonly int m_count;

        public HCSingleSelectManyTimesTest(int count) : base($"Hand-Crafted ADO.NET: SELECT 1 record, {count:N0} times")
        {
            m_count = count;
        }

        protected override void Warmup()
        {
            m_conn = TestResources.GetAdventureWorksConnection();
            m_conn.Open();
        }

        protected override void DoWork()
        {
            var rng = new Random();
            for (int i = 0; i < m_count; i++)
            {
                using (var cmd = m_conn.CreateCommand())
                {
                    cmd.CommandText = "SELECT * FROM [Production].[TransactionHistory] WHERE [TransactionID] = @Id";

                    var dbParam = cmd.CreateParameter();
                    dbParam.Direction = ParameterDirection.Input;
                    dbParam.DbType = DbType.Int32;
                    dbParam.ParameterName = "@Id";
                    dbParam.Value = rng.Next(100000, 200000);
                    cmd.Parameters.Add(dbParam);

                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            var transaction = new TransactionHistory();

                            transaction.TransactionID = reader.GetInt32(0);
                            transaction.ProductID = reader.GetInt32(1);
                            transaction.ReferenceOrderID = reader.GetInt32(2);
                            transaction.ReferenceOrderLineID = reader.GetInt32(3);
                            transaction.TransactionDate = reader.GetDateTime(4);
                            transaction.TransactionType = reader.GetString(5);
                            transaction.Quantity = reader.GetInt32(6);
                            transaction.ActualCost = reader.GetDecimal(7);
                            transaction.ModifiedDate = reader.GetDateTime(8);

                            if (transaction.ProductID <= 0)
                            {
                                throw new ApplicationException();
                            }
                        }
                        else
                        {
                            throw new ApplicationException();
                        }
                    }
                }
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (!m_disposed)
            {
                if (disposing)
                {
                    m_conn.Close();
                    m_conn.Dispose();
                }
                m_disposed = true;
            }
        }
    }
}
