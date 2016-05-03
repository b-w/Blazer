namespace Blazer.PerformanceTests.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using Models.AdventureWorks;

    public class HCSelectTest : TestBase
    {
        IDbConnection m_conn;
        readonly int m_count;

        public HCSelectTest(int count) : base($"Hand-Crafted ADO.NET: SELECT {count:N0} records")
        {
            m_count = count;
        }

        protected override void Warmup()
        {
            m_conn = TestResources.GetAdventureWorksConnection();
            m_conn.Open();
        }

        protected override void DoWork()
        {
            var transactions = new List<TransactionHistory>();
            using (var cmd = m_conn.CreateCommand())
            {
                cmd.CommandText = "SELECT TOP(@count) * FROM [Production].[TransactionHistory]";

                var dbParam = cmd.CreateParameter();
                dbParam.Direction = ParameterDirection.Input;
                dbParam.DbType = DbType.Int32;
                dbParam.ParameterName = "@count";
                dbParam.Value = m_count;
                cmd.Parameters.Add(dbParam);

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var transaction = new TransactionHistory();

                        transaction.TransactionID = reader.GetInt32(0);
                        transaction.ProductID = reader.GetInt32(1);
                        transaction.ReferenceOrderID = reader.GetInt32(2);
                        transaction.ReferenceOrderLineID = reader.GetInt32(3);
                        transaction.TransactionDate = reader.GetDateTime(4);
                        transaction.TransactionType = reader.GetString(5);
                        transaction.Quantity = reader.GetInt32(6);
                        transaction.ActualCost = reader.GetDecimal(7);
                        transaction.ModifiedDate = reader.GetDateTime(8);

                        transactions.Add(transaction);
                    }
                }
            }
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

    public class HCSingleSelectManyTimesTest : TestBase
    {
        IDbConnection m_conn;
        readonly int m_count;

        public HCSingleSelectManyTimesTest(int count) : base($"Hand-Crafted ADO.NET: SELECT 1 record, {count:N0} times")
        {
            m_count = count;
        }

        protected override void Warmup()
        {
            m_conn = TestResources.GetAdventureWorksConnection();
            m_conn.Open();
        }

        protected override void DoWork()
        {
            var rng = new Random();
            for (int i = 0; i < m_count; i++)
            {
                using (var cmd = m_conn.CreateCommand())
                {
                    cmd.CommandText = "SELECT * FROM [Production].[TransactionHistory] WHERE [TransactionID] = @Id";

                    var dbParam = cmd.CreateParameter();
                    dbParam.Direction = ParameterDirection.Input;
                    dbParam.DbType = DbType.Int32;
                    dbParam.ParameterName = "@Id";
                    dbParam.Value = rng.Next(100000, 200000);
                    cmd.Parameters.Add(dbParam);

                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            var transaction = new TransactionHistory();

                            transaction.TransactionID = reader.GetInt32(0);
                            transaction.ProductID = reader.GetInt32(1);
                            transaction.ReferenceOrderID = reader.GetInt32(2);
                            transaction.ReferenceOrderLineID = reader.GetInt32(3);
                            transaction.TransactionDate = reader.GetDateTime(4);
                            transaction.TransactionType = reader.GetString(5);
                            transaction.Quantity = reader.GetInt32(6);
                            transaction.ActualCost = reader.GetDecimal(7);
                            transaction.ModifiedDate = reader.GetDateTime(8);

                            if (transaction.ProductID <= 0)
                            {
                                throw new ApplicationException();
                            }
                        }
                        else
                        {
                            throw new ApplicationException();
                        }
                    }
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
