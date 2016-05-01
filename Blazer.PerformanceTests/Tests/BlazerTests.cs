namespace Blazer.PerformanceTests.Tests
{
    using System;
    using System.Data;
    using System.Linq;
    using Blazer;
    using Models.Blazer;

    public class BlazerSmallSelectTest : TestBase
    {
        private IDbConnection m_conn;

        public BlazerSmallSelectTest() : base("Blazer: SELECT 500 records")
        {
        }

        protected override void Warmup()
        {
            m_conn = TestResources.GetAdventureWorksConnection();
            m_conn.Open();
            var products = m_conn.Query<Product>("SELECT * FROM [Production].[Product] WHERE [ProductID] < 10")
                .ToList();
            if (products.Count == 0)
            {
                throw new ApplicationException();
            }
        }

        protected override void DoWork()
        {
            var products = m_conn.Query<Product>("SELECT * FROM [Production].[Product] WHERE [ProductID] > 10")
                .ToList();
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

    public class BlazerLargeSelectTest : TestBase
    {
        private IDbConnection m_conn;

        public BlazerLargeSelectTest() : base("Blazer: SELECT 5.000 records")
        {
        }

        protected override void Warmup()
        {
            m_conn = TestResources.GetAdventureWorksConnection();
            m_conn.Open();
            var orderDetails = m_conn.Query<PurchaseOrderDetail>("SELECT * FROM [Purchasing].[PurchaseOrderDetail] WHERE [PurchaseOrderDetailID] < 10")
                .ToList();
            if (orderDetails.Count == 0)
            {
                throw new ApplicationException();
            }
        }

        protected override void DoWork()
        {
            var orderDetails = m_conn.Query<PurchaseOrderDetail>("SELECT TOP(5000) * FROM [Purchasing].[PurchaseOrderDetail] WHERE [PurchaseOrderDetailID] > 10")
                .ToList();
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

    public class BlazerHugeSelectTest : TestBase
    {
        private IDbConnection m_conn;

        public BlazerHugeSelectTest() : base("Blazer: SELECT 50.000 records")
        {
        }

        protected override void Warmup()
        {
            m_conn = TestResources.GetAdventureWorksConnection();
            m_conn.Open();
            var transactions = m_conn.Query<TransactionHistory>("SELECT * FROM [Production].[TransactionHistory] WHERE [TransactionID] < 100010")
                .ToList();
            if (transactions.Count == 0)
            {
                throw new ApplicationException();
            }
        }

        protected override void DoWork()
        {
            var transactions = m_conn.Query<TransactionHistory>("SELECT TOP(50000) * FROM [Production].[TransactionHistory] WHERE [TransactionID] > 100010")
                .ToList();
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

    public class BlazerSingleSelectManyTimesTest : TestBase
    {
        private IDbConnection m_conn;

        public BlazerSingleSelectManyTimesTest() : base("Blazer: SELECT 1 record, 500 times")
        {
        }

        protected override void Warmup()
        {
            m_conn = TestResources.GetAdventureWorksConnection();
            m_conn.Open();
            var orderDetails = m_conn.Query<TransactionHistory>("SELECT * FROM [Production].[TransactionHistory] WHERE [TransactionID] < 100010")
                .ToList();
            if (orderDetails.Count == 0)
            {
                throw new ApplicationException();
            }
        }

        protected override void DoWork()
        {
            var rng = new Random();
            for (int i = 0; i < 500; i++)
            {
                var transaction = m_conn.FirstOrDefault<TransactionHistory>(
                    "SELECT * FROM [Production].[TransactionHistory] WHERE [TransactionID] = @Id",
                    new { Id = rng.Next(100000, 200000) });
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
