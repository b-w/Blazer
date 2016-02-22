namespace Blazer
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public static class BlazerExtensions
    {
        public static T First<T>(this IDbConnection connection, string command, object parameters = null) where T : new()
        {
            return ExecuteReader<T>(connection, command, parameters).First();
        }

        public static T FirstOrDefault<T>(this IDbConnection connection, string command, object parameters = null) where T : new()
        {
            return ExecuteReader<T>(connection, command, parameters).FirstOrDefault();
        }

        static IEnumerable<T> ExecuteReader<T>(IDbConnection connection, string command, object parameters = null) where T : new()
        {
            using (var cmd = connection.CreateCommand())
            {
                cmd.CommandText = command;

                if (parameters != null)
                {
                    ParameterFactory.AddParameters(cmd, parameters);
                }

                using (var reader = cmd.ExecuteReader())
                {
                    var mapper = DataMapperFactory.GetMapper<T>(reader);
                    while (reader.Read())
                    {
                        yield return (T)mapper(reader);
                    }
                }
            }
        }
    }
}
