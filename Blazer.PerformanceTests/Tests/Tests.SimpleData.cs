namespace Blazer.PerformanceTests.Tests
{
    using System;
    using Simple.Data;

    public class SimpleDataDynamicSelectTest : TestBase
    {
        Database m_db;
        readonly int m_count;

        public SimpleDataDynamicSelectTest(int count) : base($"Simple.Data v0.19.0: SELECT {count:N0} records (dynamic)")
        {
            m_count = count;
        }

        protected override void Warmup()
        {
            m_db = Database.OpenConnection(TestResources.CONN_ADVWORKS);
            var transactions = m_db["Production"]["TransactionHistory"].All().Take(10);
            foreach (var transaction in transactions)
            {
                if (transaction.TransactionID <= 0)
                {
                    throw new ApplicationException();
                }
            }
        }

        protected override void DoWork()
        {
            var transactions = m_db["Production"]["TransactionHistory"].All().Take(m_count);
            foreach (var transaction in transactions)
            {
                if (transaction.TransactionID <= 0)
                {
                    throw new ApplicationException();
                }
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (!m_disposed)
            {
                m_disposed = true;
            }
        }
    }

    public class SimpleDataSingleDynamicSelectManyTimesTest : TestBase
    {
        Database m_db;
        readonly int m_count;

        public SimpleDataSingleDynamicSelectManyTimesTest(int count) : base($"Simple.Data v0.19.0: SELECT 1 record, {count:N0} times (dynamic)")
        {
            m_count = count;
        }

        protected override void Warmup()
        {
            m_db = Database.OpenConnection(TestResources.CONN_ADVWORKS);
            var transaction = m_db["Production"]["TransactionHistory"].FindByTransactionID(100000);
            if (transaction.ProductID <= 0)
            {
                throw new ApplicationException();
            }
        }

        protected override void DoWork()
        {
            var rng = new Random();
            for (int i = 0; i < m_count; i++)
            {
                var transaction = m_db["Production"]["TransactionHistory"].FindByTransactionID(rng.Next(100000, 200000));
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
                m_disposed = true;
            }
        }
    }
}
