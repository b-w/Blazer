namespace Blazer.PerformanceTests.Tests
{
    using System;
    using System.Data;
    using System.Linq;
    using Models.AdventureWorks;
    using PetaPoco;

    public class PetaPocoSmallSelectTest : TestBase
    {
        IDbConnection m_conn;
        Database m_db;

        public PetaPocoSmallSelectTest() : base("PetaPoco v5.1.1.171: SELECT 500 records")
        {
        }

        protected override void Warmup()
        {
            m_conn = TestResources.GetAdventureWorksConnection();
            m_conn.Open();
            m_db = new Database(m_conn);
            var products = m_db.Query<Product>("SELECT * FROM [Production].[Product] WHERE [ProductID] < 10")
                .ToList();
            if (products.Count == 0)
            {
                throw new ApplicationException();
            }
        }

        protected override void DoWork()
        {
            var products = m_db.Query<Product>("SELECT * FROM [Production].[Product] WHERE [ProductID] > 10")
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

    public class PetaPocoLargeSelectTest : TestBase
    {
        IDbConnection m_conn;
        Database m_db;

        public PetaPocoLargeSelectTest() : base("PetaPoco v5.1.1.171: SELECT 5.000 records")
        {
        }

        protected override void Warmup()
        {
            m_conn = TestResources.GetAdventureWorksConnection();
            m_conn.Open();
            m_db = new Database(m_conn);
            var orderDetails = m_db.Query<PurchaseOrderDetail>("SELECT * FROM [Purchasing].[PurchaseOrderDetail] WHERE [PurchaseOrderDetailID] < 10")
                .ToList();
            if (orderDetails.Count == 0)
            {
                throw new ApplicationException();
            }
        }

        protected override void DoWork()
        {
            var orderDetails = m_db.Query<PurchaseOrderDetail>("SELECT TOP(5000) * FROM [Purchasing].[PurchaseOrderDetail] WHERE [PurchaseOrderDetailID] > 10")
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

    public class PetaPocoHugeSelectTest : TestBase
    {
        IDbConnection m_conn;
        Database m_db;

        public PetaPocoHugeSelectTest() : base("PetaPoco v5.1.1.171: SELECT 50.000 records")
        {
        }

        protected override void Warmup()
        {
            m_conn = TestResources.GetAdventureWorksConnection();
            m_conn.Open();
            m_db = new Database(m_conn);
            var transactions = m_db.Query<TransactionHistory>("SELECT * FROM [Production].[TransactionHistory] WHERE [TransactionID] < 100010")
                .ToList();
            if (transactions.Count == 0)
            {
                throw new ApplicationException();
            }
        }

        protected override void DoWork()
        {
            var transactions = m_db.Query<TransactionHistory>("SELECT TOP(50000) * FROM [Production].[TransactionHistory] WHERE [TransactionID] > 100010")
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

    public class PetaPocoSingleSelectManyTimesTest : TestBase
    {
        IDbConnection m_conn;
        Database m_db;
        readonly int m_count;

        public PetaPocoSingleSelectManyTimesTest(int count) : base($"PetaPoco v5.1.1.171: SELECT 1 record, {count:N0} times")
        {
            m_count = count;
        }

        protected override void Warmup()
        {
            m_conn = TestResources.GetAdventureWorksConnection();
            m_conn.Open();
            m_db = new Database(m_conn);
            var orderDetails = m_db.Query<TransactionHistory>("SELECT * FROM [Production].[TransactionHistory] WHERE [TransactionID] < 100010")
                .ToList();
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
                var transaction = m_db.FirstOrDefault<TransactionHistory>(
                    "SELECT * FROM [Production].[TransactionHistory] WHERE [TransactionID] = @0",
                    rng.Next(100000, 200000));
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

    public class PetaPocoSingleDynamicSelectManyTimesTest : TestBase
    {
        IDbConnection m_conn;
        Database m_db;
        readonly int m_count;

        public PetaPocoSingleDynamicSelectManyTimesTest(int count) : base($"PetaPoco v5.1.1.171: SELECT 1 record, {count:N0} times (dynamic)")
        {
            m_count = count;
        }

        protected override void Warmup()
        {
            m_conn = TestResources.GetAdventureWorksConnection();
            m_conn.Open();
            m_db = new Database(m_conn);
            var orderDetails = m_db.Query<dynamic>("SELECT * FROM [Production].[TransactionHistory] WHERE [TransactionID] < 100010")
                .ToList();
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
                var transaction = m_db.FirstOrDefault<dynamic>(
                    "SELECT * FROM [Production].[TransactionHistory] WHERE [TransactionID] = @0",
                    rng.Next(100000, 200000));
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
