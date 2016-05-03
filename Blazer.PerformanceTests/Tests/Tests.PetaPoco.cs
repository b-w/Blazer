namespace Blazer.PerformanceTests.Tests
{
    using System;
    using System.Data;
    using System.Linq;
    using Models.AdventureWorks;
    using PetaPoco;

    public class PetaPocoSelectTest : TestBase
    {
        IDbConnection m_conn;
        Database m_db;
        readonly int m_count;

        public PetaPocoSelectTest(int count) : base($"PetaPoco v5.1.1.171: SELECT {count:N0} records")
        {
            m_count = count;
        }

        protected override void Warmup()
        {
            m_conn = TestResources.GetAdventureWorksConnection();
            m_conn.Open();
            m_db = new Database(m_conn);
            var transactions = m_db.Query<TransactionHistory>("SELECT TOP(@0) * FROM [Production].[TransactionHistory]", 10)
                .ToList();
            if (transactions.Count == 0)
            {
                throw new ApplicationException();
            }
        }

        protected override void DoWork()
        {
            var transactions = m_db.Query<TransactionHistory>("SELECT TOP(@0) * FROM [Production].[TransactionHistory]", m_count)
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
