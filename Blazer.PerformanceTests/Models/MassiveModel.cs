namespace Blazer.PerformanceTests.Models
{
    using Massive;

    public class MassiveTransactionHistory : DynamicModel
    {
        public MassiveTransactionHistory() : base("AdventureWorksDB", "Production.TransactionHistory", "TransactionID")
        {
        }
    }
}
