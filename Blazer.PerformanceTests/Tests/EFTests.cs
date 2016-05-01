namespace Blazer.PerformanceTests.Tests
{
    using System;
    using System.Linq;
    using Models;

    public class EFSmallSelectTest : TestBase
    {
        private EFEntities m_ctx;

        public EFSmallSelectTest() : base("Entity Framework: SELECT 500 records")
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

    public class EFLargeSelectTest : TestBase
    {
        private EFEntities m_ctx;

        public EFLargeSelectTest() : base("Entity Framework: SELECT 5.000 records")
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

    public class EFHugeSelectTest : TestBase
    {
        private EFEntities m_ctx;

        public EFHugeSelectTest() : base("Entity Framework: SELECT 50.000 records")
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

    public class EFSingleSelectManyTimesTest : TestBase
    {
        private EFEntities m_ctx;

        public EFSingleSelectManyTimesTest() : base("Entity Framework: SELECT 1 record, 500 times")
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
            var rng = new Random();
            for (int i = 0; i < 500; i++)
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
