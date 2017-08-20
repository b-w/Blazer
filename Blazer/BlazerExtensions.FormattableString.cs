#if FEATURE_FORMATTABLE_STRING
namespace Blazer
{
    using System;
    using System.Collections.Generic;
    using System.Data;

    public static partial class BlazerExtensions
    {
        public static T Scalar<T>(this IDbConnection connection, FormattableString command, CommandConfiguration config = null)
        {
            using (var cmd = CommandFactory.CreateCommand(connection, command.Format, config))
            {
                if (command.ArgumentCount > 0)
                {
                    ParameterFactory.AddParameters(cmd, command);
                }

                var value = cmd.ExecuteScalar();
                if (value == null || value == DBNull.Value)
                {
                    return default(T);
                }
                return (T)value;
            }
        }

        public static int Command(this IDbConnection connection, FormattableString command, CommandConfiguration config = null)
        {
            using (var cmd = CommandFactory.CreateCommand(connection, command.Format, config))
            {
                if (command.ArgumentCount > 0)
                {
                    ParameterFactory.AddParameters(cmd, command);
                }

                return cmd.ExecuteNonQuery();
            }
        }

        public static IEnumerable<T> Query<T>(this IDbConnection connection, FormattableString command, CommandConfiguration config = null) where T : new()
        {
            using (var cmd = CommandFactory.CreateCommand(connection, command.Format, config))
            {
                if (command.ArgumentCount > 0)
                {
                    ParameterFactory.AddParameters(cmd, command);
                }

                using (var reader = cmd.ExecuteReader(s_defaultCommandBehavior))
                {
                    var mapper = DataMapperFactory.GetMapper<T>(cmd, reader);
                    while (reader.Read())
                    {
                        yield return (T)mapper(reader);
                    }
                }
            }
        }

        public static T QuerySingle<T>(this IDbConnection connection, FormattableString command, CommandConfiguration config = null) where T : new()
        {
            using (var cmd = CommandFactory.CreateCommand(connection, command.Format, config))
            {
                if (command.ArgumentCount > 0)
                {
                    ParameterFactory.AddParameters(cmd, command);
                }

                using (var reader = cmd.ExecuteReader(s_defaultCommandBehavior | CommandBehavior.SingleRow))
                {
                    var mapper = DataMapperFactory.GetMapper<T>(cmd, reader);
                    if (reader.Read())
                    {
                        return (T)mapper(reader);
                    }
                    return default(T);
                }
            }
        }

        public static IEnumerable<dynamic> Query(this IDbConnection connection, FormattableString command, CommandConfiguration config = null)
        {
            using (var cmd = CommandFactory.CreateCommand(connection, command.Format, config))
            {
                if (command.ArgumentCount > 0)
                {
                    ParameterFactory.AddParameters(cmd, command);
                }

                using (var reader = cmd.ExecuteReader(s_defaultCommandBehavior))
                {
                    var mapper = DataMapperFactory.GetMapper(cmd, reader);
                    while (reader.Read())
                    {
                        yield return mapper(reader);
                    }
                }
            }
        }

        public static dynamic QuerySingle(this IDbConnection connection, FormattableString command, CommandConfiguration config = null)
        {
            using (var cmd = CommandFactory.CreateCommand(connection, command.Format, config))
            {
                if (command.ArgumentCount > 0)
                {
                    ParameterFactory.AddParameters(cmd, command);
                }

                using (var reader = cmd.ExecuteReader(s_defaultCommandBehavior | CommandBehavior.SingleRow))
                {
                    var mapper = DataMapperFactory.GetMapper(cmd, reader);
                    if (reader.Read())
                    {
                        return mapper(reader);
                    }
                    return null;
                }
            }
        }
    }
}
#endif
