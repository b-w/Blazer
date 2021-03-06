﻿namespace Blazer.Tests
{
    using System.Data;
    using System.Data.SqlClient;

    internal static class TestResources
    {
        public const string CONN_ADVWORKS = @"Server=.\SQLEXPRESS;Database=AdventureWorks;Integrated Security=True";

        public static IDbConnection GetAdventureWorksConnection()
        {
            return new SqlConnection(CONN_ADVWORKS);
        }
    }
}
