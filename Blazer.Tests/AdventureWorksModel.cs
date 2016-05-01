namespace Blazer.Tests
{
    using System;

    public class AWProduct
    {
        public int ProductID { get; set; }

        public string Name { get; set; }

        public string Color { get; set; }

        public bool MakeFlag { get; set; }

        public short ReorderPoint { get; set; }

        public int DaysToManufacture { get; set; }

        public int? ProductModelID { get; set; }

        public decimal ListPrice { get; set; }

        public DateTime SellStartDate { get; set; }

        public DateTime? SellEndDate { get; set; }

        public Guid rowguid { get; set; }
    }

    public class AWDocument
    {
        public string Title { get; set; }

        public bool FolderFlag { get; set; }

        public byte Status { get; set; }

        public byte[] Document { get; set; }
    }

    public class AWSalesPerson
    {
        public int BusinessEntityID { get; set; }

        public decimal? SalesQuota { get; set; }
    }

    public class AWEmployee
    {
        public int BusinessEntityID { get; set; }

        public char Gender { get; set; }
    }

    public class AWEmployeeManager
    {
        public int RecursionLevel { get; set; }

        public int BusinessEntityID { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string OrganizationNode { get; set; }

        public string ManagerFirstName { get; set; }

        public string ManagerLastName { get; set; }
    }
}
