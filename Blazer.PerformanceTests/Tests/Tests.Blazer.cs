namespace Blazer.PerformanceTests.Tests
{
    using System;
    using System.Data;
    using System.Linq;
    using Blazer;
    using Models.AdventureWorks;

    public class BlazerSelectTest : TestBase
    {
        IDbConnection m_conn;
        readonly int m_count;

        public BlazerSelectTest(int count) : base($"Blazer: SELECT {count:N0} records")
        {
            m_count = count;
        }

        protected override void Warmup()
        {
            m_conn = TestResources.GetAdventureWorksConnection();
            m_conn.Open();
            var transactions = m_conn.Query<TransactionHistory>("SELECT TOP(@count) * FROM [Production].[TransactionHistory]", new { count = 10 })
                .ToList();
            if (transactions.Count == 0)
            {
                throw new ApplicationException();
            }
        }

        protected override void DoWork()
        {
            var transactions = m_conn.Query<TransactionHistory>("SELECT TOP(@count) * FROM [Production].[TransactionHistory]", new { count = m_count })
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

    public class BlazerDynamicSelectTest : TestBase
    {
        IDbConnection m_conn;
        readonly int m_count;

        public BlazerDynamicSelectTest(int count) : base($"Blazer: SELECT {count:N0} records (dynamic)")
        {
            m_count = count;
        }

        protected override void Warmup()
        {
            m_conn = TestResources.GetAdventureWorksConnection();
            m_conn.Open();
            var transactions = m_conn.Query("SELECT TOP(@count) * FROM [Production].[TransactionHistory]", new { count = 10 })
                .ToList();
            if (transactions.Count == 0)
            {
                throw new ApplicationException();
            }
        }

        protected override void DoWork()
        {
            var transactions = m_conn.Query("SELECT TOP(@count) * FROM [Production].[TransactionHistory]", new { count = m_count })
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
        IDbConnection m_conn;
        readonly int m_count;

        public BlazerSingleSelectManyTimesTest(int count) : base($"Blazer: SELECT 1 record, {count:N0} times")
        {
            m_count = count;
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
            for (int i = 0; i < m_count; i++)
            {
                var transaction = m_conn.QuerySingle<TransactionHistory>(
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

    public class BlazerSingleDynamicSelectManyTimesTest : TestBase
    {
        IDbConnection m_conn;
        readonly int m_count;

        public BlazerSingleDynamicSelectManyTimesTest(int count) : base($"Blazer: SELECT 1 record, {count:N0} times (dynamic)")
        {
            m_count = count;
        }

        protected override void Warmup()
        {
            m_conn = TestResources.GetAdventureWorksConnection();
            m_conn.Open();
            var orderDetails = m_conn.Query("SELECT * FROM [Production].[TransactionHistory] WHERE [TransactionID] < 100010")
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
                var transaction = m_conn.QuerySingle(
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
