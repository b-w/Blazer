namespace Blazer
{
    using System.Data;
    using System.Linq;

    public static class BlazerLinqExtensions
    {
        public static T First<T>(this IDbConnection connection, string command, object parameters = null) where T : new()
        {
            return connection.Query<T>(command, parameters).First();
        }

        public static T FirstOrDefault<T>(this IDbConnection connection, string command, object parameters = null) where T : new()
        {
            return connection.Query<T>(command, parameters).FirstOrDefault();
        }

        public static T Single<T>(this IDbConnection connection, string command, object parameters = null) where T : new()
        {
            return connection.Query<T>(command, parameters).Single();
        }

        public static T SingleOrDefault<T>(this IDbConnection connection, string command, object parameters = null) where T : new()
        {
            return connection.Query<T>(command, parameters).SingleOrDefault();
        }
    }
}
