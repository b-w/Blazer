namespace Blazer.PerformanceTests.Tests
{
    using System;
    using System.Linq;
    using Models.L2S;

    public class L2SSmallSelectTest : TestBase
    {
        L2SModelDataContext m_ctx;

        public L2SSmallSelectTest() : base("Linq2SQL: SELECT 500 records")
        {
        }

        protected override void Warmup()
        {
            m_ctx = new L2SModelDataContext();
            m_ctx.ObjectTrackingEnabled = false;
            var products = m_ctx.Products
                .Where(x => x.ProductID < 10)
                .ToList();
            if (products.Count == 0)
            {
                throw new ApplicationException();
            }
        }

        protected override void DoWork()
        {
            var products = m_ctx.Products
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

    public class L2SLargeSelectTest : TestBase
    {
        L2SModelDataContext m_ctx;

        public L2SLargeSelectTest() : base("Linq2SQL: SELECT 5.000 records")
        {
        }

        protected override void Warmup()
        {
            m_ctx = new L2SModelDataContext();
            m_ctx.ObjectTrackingEnabled = false;
            var orderDetails = m_ctx.PurchaseOrderDetails
                .Where(x => x.PurchaseOrderDetailID < 10)
                .ToList();
            if (orderDetails.Count == 0)
            {
                throw new ApplicationException();
            }
        }

        protected override void DoWork()
        {
            var orderDetails = m_ctx.PurchaseOrderDetails
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

    public class L2SHugeSelectTest : TestBase
    {
        L2SModelDataContext m_ctx;

        public L2SHugeSelectTest() : base("Linq2SQL: SELECT 50.000 records")
        {
        }

        protected override void Warmup()
        {
            m_ctx = new L2SModelDataContext();
            m_ctx.ObjectTrackingEnabled = false;
            var transactions = m_ctx.TransactionHistories
                .Where(x => x.TransactionID < 100010)
                .ToList();
            if (transactions.Count == 0)
            {
                throw new ApplicationException();
            }
        }

        protected override void DoWork()
        {
            var transactions = m_ctx.TransactionHistories
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

    public class L2SSingleSelectManyTimesTest : TestBase
    {
        L2SModelDataContext m_ctx;
        readonly int m_count;

        public L2SSingleSelectManyTimesTest(int count) : base($"Linq2SQL: SELECT 1 record, {count:N0} times")
        {
            m_count = count;
        }

        protected override void Warmup()
        {
            m_ctx = new L2SModelDataContext();
            m_ctx.ObjectTrackingEnabled = false;
            var transactions = m_ctx.TransactionHistories
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
                var transaction = m_ctx.TransactionHistories
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
