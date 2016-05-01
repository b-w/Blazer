namespace Blazer
{
    using System.Data;
    using System.Linq;

    public static class BlazerLinqExtensions
    {
        public static T First<T>(this IDbConnection connection, string command, object parameters = null, CommandConfiguration config = null) where T : new()
        {
            return connection.Query<T>(command, parameters, config).First();
        }

        public static dynamic First(this IDbConnection connection, string command, object parameters = null, CommandConfiguration config = null)
        {
            return connection.Query(command, parameters, config).First();
        }

        public static T FirstOrDefault<T>(this IDbConnection connection, string command, object parameters = null, CommandConfiguration config = null) where T : new()
        {
            return connection.Query<T>(command, parameters, config).FirstOrDefault();
        }

        public static dynamic FirstOrDefault(this IDbConnection connection, string command, object parameters = null, CommandConfiguration config = null)
        {
            return connection.Query(command, parameters, config).FirstOrDefault();
        }

        public static T Single<T>(this IDbConnection connection, string command, object parameters = null, CommandConfiguration config = null) where T : new()
        {
            return connection.Query<T>(command, parameters, config).Single();
        }

        public static dynamic Single(this IDbConnection connection, string command, object parameters = null, CommandConfiguration config = null)
        {
            return connection.Query(command, parameters, config).Single();
        }

        public static T SingleOrDefault<T>(this IDbConnection connection, string command, object parameters = null, CommandConfiguration config = null) where T : new()
        {
            return connection.Query<T>(command, parameters, config).SingleOrDefault();
        }

        public static dynamic SingleOrDefault(this IDbConnection connection, string command, object parameters = null, CommandConfiguration config = null)
        {
            return connection.Query(command, parameters, config).SingleOrDefault();
        }
    }
}
