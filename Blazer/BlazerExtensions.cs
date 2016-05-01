namespace Blazer
{
    using System;
    using System.Collections.Generic;
    using System.Data;

    public static class BlazerExtensions
    {
        public static T Scalar<T>(this IDbConnection connection, string command, object parameters = null, CommandConfiguration config = null)
        {
            using (var cmd = CommandFactory.CreateCommand(connection, command, config))
            {
                if (parameters != null)
                {
                    ParameterFactory.AddParameters(cmd, parameters);
                }

                var value = cmd.ExecuteScalar();
                if (value == null || value == DBNull.Value)
                {
                    return default(T);
                }
                return (T)value;
            }
        }

        public static int Command(this IDbConnection connection, string command, object parameters = null, CommandConfiguration config = null)
        {
            using (var cmd = CommandFactory.CreateCommand(connection, command, config))
            {
                if (parameters != null)
                {
                    ParameterFactory.AddParameters(cmd, parameters);
                }

                return cmd.ExecuteNonQuery();
            }
        }

        public static IEnumerable<T> Query<T>(this IDbConnection connection, string command, object parameters = null, CommandConfiguration config = null) where T : new()
        {
            using (var cmd = CommandFactory.CreateCommand(connection, command, config))
            {
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

        public static IEnumerable<dynamic> Query(this IDbConnection connection, string command, object parameters = null, CommandConfiguration config = null)
        {
            using (var cmd = CommandFactory.CreateCommand(connection, command, config))
            {
                if (parameters != null)
                {
                    ParameterFactory.AddParameters(cmd, parameters);
                }

                using (var reader = cmd.ExecuteReader())
                {
                    var mapper = DataMapperFactory.GetMapper();
                    while (reader.Read())
                    {
                        yield return mapper(reader);
                    }
                }
            }
        }
    }
}
