namespace Blazer.PerformanceTests.Tests
{
    using System;
    using System.Linq;
    using Models.Entity;

    public class EFSelectTest : TestBase
    {
        EFEntities m_ctx;
        readonly int m_count;

        public EFSelectTest(int count) : base($"EF v6.1.3: SELECT {count:N0} records")
        {
            m_count = count;
        }

        protected override void Warmup()
        {
            m_ctx = new EFEntities();
            var transactions = m_ctx.TransactionHistory
                .AsNoTracking()
                .Take(10)
                .ToList();
            if (transactions.Count == 0)
            {
                throw new ApplicationException();
            }
        }

        protected override void DoWork()
        {
            var transactions = m_ctx.TransactionHistory
                .AsNoTracking()
                .Take(m_count)
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
                    m_ctx.Dispose();
                }
                m_disposed = true;
            }
        }
    }

    public class EFSingleSelectManyTimesTest : TestBase
    {
        EFEntities m_ctx;
        readonly int m_count;

        public EFSingleSelectManyTimesTest(int count) : base($"EF v6.1.3: SELECT 1 record, {count:N0} times")
        {
            m_count = count;
        }

        protected override void Warmup()
        {
            m_ctx = new EFEntities();
            var transactions = m_ctx.TransactionHistory
                .AsNoTracking()
                .Where(x => x.TransactionID < 100010)
                .ToList();
            if (transactions.Count == 0)
            {
                throw new ApplicationException();
            }
        }

        protected override void DoWork()
        {
            var rng = new Random();
            for (int i = 0; i < m_count; i++)
            {
                var id = rng.Next(100000, 200000);
                var transaction = m_ctx.TransactionHistory
                    .AsNoTracking()
                    .Where(x => x.TransactionID == id)
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
                    m_ctx.Dispose();
                }
                m_disposed = true;
            }
        }
    }
}
