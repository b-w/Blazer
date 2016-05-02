namespace Blazer.PerformanceTests.Tests
{
    using System;
    using System.Data;
    using System.Linq;
    using Models.AdventureWorks;
    using ServiceStack.OrmLite;

    public class OrmLiteSmallSelectTest : TestBase
    {
        IDbConnection m_conn;

        public OrmLiteSmallSelectTest() : base("OrmLite v4.0.56: SELECT 500 records")
        {
        }

        protected override void Warmup()
        {
            var dbFactory = new OrmLiteConnectionFactory(TestResources.CONN_ADVWORKS, SqlServerDialect.Provider);
            m_conn = dbFactory.OpenDbConnection();
            var products = m_conn.SqlList<Product>("SELECT * FROM [Production].[Product] WHERE [ProductID] < 10");
            if (products.Count == 0)
            {
                throw new ApplicationException();
            }
        }

        protected override void DoWork()
        {
            var products = m_conn.SqlList<Product>("SELECT * FROM [Production].[Product] WHERE [ProductID] > 10");
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

    public class OrmLiteLargeSelectTest : TestBase
    {
        IDbConnection m_conn;

        public OrmLiteLargeSelectTest() : base("OrmLite v4.0.56: SELECT 5.000 records")
        {
        }

        protected override void Warmup()
        {
            var dbFactory = new OrmLiteConnectionFactory(TestResources.CONN_ADVWORKS, SqlServerDialect.Provider);
            m_conn = dbFactory.OpenDbConnection();
            var orderDetails = m_conn.SqlList<PurchaseOrderDetail>("SELECT * FROM [Purchasing].[PurchaseOrderDetail] WHERE [PurchaseOrderDetailID] < 10");
            if (orderDetails.Count == 0)
            {
                throw new ApplicationException();
            }
        }

        protected override void DoWork()
        {
            var orderDetails = m_conn.SqlList<PurchaseOrderDetail>("SELECT TOP(5000) * FROM [Purchasing].[PurchaseOrderDetail] WHERE [PurchaseOrderDetailID] > 10");
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

    public class OrmLiteHugeSelectTest : TestBase
    {
        IDbConnection m_conn;

        public OrmLiteHugeSelectTest() : base("OrmLite v4.0.56: SELECT 50.000 records")
        {
        }

        protected override void Warmup()
        {
            var dbFactory = new OrmLiteConnectionFactory(TestResources.CONN_ADVWORKS, SqlServerDialect.Provider);
            m_conn = dbFactory.OpenDbConnection();
            var transactions = m_conn.SqlList<TransactionHistory>("SELECT * FROM [Production].[TransactionHistory] WHERE [TransactionID] < 100010");
            if (transactions.Count == 0)
            {
                throw new ApplicationException();
            }
        }

        protected override void DoWork()
        {
            var transactions = m_conn.SqlList<TransactionHistory>("SELECT TOP(50000) * FROM [Production].[TransactionHistory] WHERE [TransactionID] > 100010");
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

    public class OrmLiteSingleSelectManyTimesTest : TestBase
    {
        IDbConnection m_conn;
        readonly int m_count;

        public OrmLiteSingleSelectManyTimesTest(int count) : base($"OrmLite v4.0.56: SELECT 1 record, {count:N0} times")
        {
            m_count = count;
        }

        protected override void Warmup()
        {
            var dbFactory = new OrmLiteConnectionFactory(TestResources.CONN_ADVWORKS, SqlServerDialect.Provider);
            m_conn = dbFactory.OpenDbConnection();
            var orderDetails = m_conn.SqlList<TransactionHistory>("SELECT * FROM [Production].[TransactionHistory] WHERE [TransactionID] < 100010");
            if (orderDetails.Count == 0)
            {
                throw new ApplicationException();
            }
        }

        protected override void DoWork()
        {
            var rng = new Random();
            for (int i = 0; i < m_count; i++)
            {
                var transaction = m_conn.SqlList<TransactionHistory>(
                        "SELECT * FROM [Production].[TransactionHistory] WHERE [TransactionID] = @Id",
                        new { Id = rng.Next(100000, 200000) })
                    .FirstOrDefault();
                if (transaction != null && transaction.ProductID <= 0)
                {
                    throw new ApplicationException();
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
