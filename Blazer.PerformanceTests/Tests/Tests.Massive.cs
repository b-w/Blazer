namespace Blazer.PerformanceTests.Tests
{
    using System;
    using Models;

    public class MassiveDynamicSelectTest : TestBase
    {
        MassiveTransactionHistory m_db;
        readonly int m_count;

        public MassiveDynamicSelectTest(int count) : base($"Massive v2.0: SELECT {count:N0} records (dynamic)")
        {
            m_count = count;
        }

        protected override void Warmup()
        {
            m_db = new MassiveTransactionHistory();
            var transactions = m_db.Paged(pageSize: 10);
            foreach (var transaction in transactions.Items)
            {
                if (transaction.TransactionID <= 0)
                {
                    throw new ApplicationException();
                }
            }
        }

        protected override void DoWork()
        {
            var transactions = m_db.Paged(pageSize: m_count);
            foreach (var transaction in transactions.Items)
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

    public class MassiveSingleDynamicSelectManyTimesTest : TestBase
    {
        MassiveTransactionHistory m_db;
        readonly int m_count;

        public MassiveSingleDynamicSelectManyTimesTest(int count) : base($"Massive v2.0: SELECT 1 record, {count:N0} times (dynamic)")
        {
            m_count = count;
        }

        protected override void Warmup()
        {
            m_db = new MassiveTransactionHistory();
            var transaction = m_db.Single("WHERE TransactionID = @0", 100000);
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
                var transaction = m_db.Single("WHERE TransactionID = @0", rng.Next(100000, 200000));
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
