namespace Blazer.PerformanceTests.Tests
{
    using System;
    using System.Data;
    using System.Linq;
    using Models.AdventureWorks;
    using ServiceStack.OrmLite;

    public class OrmLiteSelectTest : TestBase
    {
        IDbConnection m_conn;
        readonly int m_count;

        public OrmLiteSelectTest(int count) : base($"OrmLite v4.0.56: SELECT {count:N0} records")
        {
            m_count = count;
        }

        protected override void Warmup()
        {
            var dbFactory = new OrmLiteConnectionFactory(TestResources.CONN_ADVWORKS, SqlServerDialect.Provider);
            m_conn = dbFactory.OpenDbConnection();
            var transactions = m_conn.SqlList<TransactionHistory>("SELECT TOP(@count) * FROM [Production].[TransactionHistory]", new { count = 10 });
            if (transactions.Count == 0)
            {
                throw new ApplicationException();
            }
        }

        protected override void DoWork()
        {
            var transactions = m_conn.SqlList<TransactionHistory>("SELECT TOP(@count) * FROM [Production].[TransactionHistory]", new { count = m_count });
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
