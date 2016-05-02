namespace Blazer.PerformanceTests.Tests
{
    using System;
    using System.Linq;
    using Models.Entity;

    public class EFCCSmallSelectTest : TestBase
    {
        EFEntities m_ctx;

        public EFCCSmallSelectTest() : base("EF v6.1.3 (change tracking): SELECT 500 records")
        {
        }

        protected override void Warmup()
        {
            m_ctx = new EFEntities();
            var products = m_ctx.Product
                .Where(x => x.ProductID < 10)
                .ToList();
            if (products.Count == 0)
            {
                throw new ApplicationException();
            }
        }

        protected override void DoWork()
        {
            var products = m_ctx.Product
                .Where(x => x.ProductID > 10)
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
                    m_ctx.Dispose();
                }
                m_disposed = true;
            }
        }
    }

    public class EFCCLargeSelectTest : TestBase
    {
        EFEntities m_ctx;

        public EFCCLargeSelectTest() : base("EF v6.1.3 (change tracking): SELECT 5.000 records")
        {
        }

        protected override void Warmup()
        {
            m_ctx = new EFEntities();
            var orderDetails = m_ctx.PurchaseOrderDetail
                .Where(x => x.PurchaseOrderDetailID < 10)
                .ToList();
            if (orderDetails.Count == 0)
            {
                throw new ApplicationException();
            }
        }

        protected override void DoWork()
        {
            var orderDetails = m_ctx.PurchaseOrderDetail
                .Where(x => x.PurchaseOrderDetailID > 10)
                .Take(5000)
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
                    m_ctx.Dispose();
                }
                m_disposed = true;
            }
        }
    }

    public class EFCCHugeSelectTest : TestBase
    {
        EFEntities m_ctx;

        public EFCCHugeSelectTest() : base("EF v6.1.3 (change tracking): SELECT 50.000 records")
        {
        }

        protected override void Warmup()
        {
            m_ctx = new EFEntities();
            var transactions = m_ctx.TransactionHistory
                .Where(x => x.TransactionID < 100010)
                .ToList();
            if (transactions.Count == 0)
            {
                throw new ApplicationException();
            }
        }

        protected override void DoWork()
        {
            var transactions = m_ctx.TransactionHistory
                .Where(x => x.TransactionID > 100010)
                .Take(50000)
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

    public class EFCCSingleSelectManyTimesTest : TestBase
    {
        EFEntities m_ctx;
        readonly int m_count;

        public EFCCSingleSelectManyTimesTest(int count) : base($"EF v6.1.3 (change tracking): SELECT 1 record, {count:N0} times")
        {
            m_count = count;
        }

        protected override void Warmup()
        {
            m_ctx = new EFEntities();
            var transactions = m_ctx.TransactionHistory
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
