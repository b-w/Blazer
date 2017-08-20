namespace Blazer
{
    using System;
    using System.Collections.Generic;
    using System.Data;

    public static partial class BlazerExtensions
    {
        const CommandBehavior s_defaultCommandBehavior = CommandBehavior.SequentialAccess | CommandBehavior.SingleResult;

#if FEATURE_FORMATTABLE_STRING
        public static T Scalar<T>(this IDbConnection connection, NonFormattableString command, object parameters = null, CommandConfiguration config = null)
        {
            using (var cmd = CommandFactory.CreateCommand(connection, command.Value, config))
            {
#else
        public static T Scalar<T>(this IDbConnection connection, string command, object parameters = null, CommandConfiguration config = null)
        {
            using (var cmd = CommandFactory.CreateCommand(connection, command, config))
            {
#endif
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

#if FEATURE_FORMATTABLE_STRING
        public static int Command(this IDbConnection connection, NonFormattableString command, object parameters = null, CommandConfiguration config = null)
        {
            using (var cmd = CommandFactory.CreateCommand(connection, command.Value, config))
            {
#else
        public static int Command(this IDbConnection connection, string command, object parameters = null, CommandConfiguration config = null)
        {
            using (var cmd = CommandFactory.CreateCommand(connection, command, config))
            {
#endif
                if (parameters != null)
                {
                    ParameterFactory.AddParameters(cmd, parameters);
                }

                return cmd.ExecuteNonQuery();
            }
        }

#if FEATURE_FORMATTABLE_STRING
        public static IEnumerable<T> Query<T>(this IDbConnection connection, NonFormattableString command, object parameters = null, CommandConfiguration config = null) where T : new()
        {
            using (var cmd = CommandFactory.CreateCommand(connection, command.Value, config))
            {
#else
        public static IEnumerable<T> Query<T>(this IDbConnection connection, string command, object parameters = null, CommandConfiguration config = null) where T : new()
        {
            using (var cmd = CommandFactory.CreateCommand(connection, command, config))
            {
#endif
                if (parameters != null)
                {
                    ParameterFactory.AddParameters(cmd, parameters);
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

#if FEATURE_FORMATTABLE_STRING
        public static T QuerySingle<T>(this IDbConnection connection, NonFormattableString command, object parameters = null, CommandConfiguration config = null) where T : new()
        {
            using (var cmd = CommandFactory.CreateCommand(connection, command.Value, config))
            {
#else
        public static T QuerySingle<T>(this IDbConnection connection, string command, object parameters = null, CommandConfiguration config = null) where T : new()
        {
            using (var cmd = CommandFactory.CreateCommand(connection, command, config))
            {
#endif
                if (parameters != null)
                {
                    ParameterFactory.AddParameters(cmd, parameters);
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

#if FEATURE_FORMATTABLE_STRING
        public static IEnumerable<dynamic> Query(this IDbConnection connection, NonFormattableString command, object parameters = null, CommandConfiguration config = null)
        {
            using (var cmd = CommandFactory.CreateCommand(connection, command.Value, config))
            {
#else
        public static IEnumerable<dynamic> Query(this IDbConnection connection, string command, object parameters = null, CommandConfiguration config = null)
        {
            using (var cmd = CommandFactory.CreateCommand(connection, command, config))
            {
#endif
                if (parameters != null)
                {
                    ParameterFactory.AddParameters(cmd, parameters);
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

#if FEATURE_FORMATTABLE_STRING
        public static dynamic QuerySingle(this IDbConnection connection, NonFormattableString command, object parameters = null, CommandConfiguration config = null)
        {
            using (var cmd = CommandFactory.CreateCommand(connection, command.Value, config))
            {
#else
        public static dynamic QuerySingle(this IDbConnection connection, string command, object parameters = null, CommandConfiguration config = null)
        {
            using (var cmd = CommandFactory.CreateCommand(connection, command, config))
            {
#endif
                if (parameters != null)
                {
                    ParameterFactory.AddParameters(cmd, parameters);
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

        public static void StoredProcedure(this IDbConnection connection, string command, SpParameters parameters = null, CommandConfiguration config = null)
        {
            using (var cmd = CommandFactory.CreateCommand(connection, command, config))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                var paramPairs = new List<Tuple<SpParameter, IDbDataParameter>>();
                if (parameters != null)
                {
                    foreach (var spParam in parameters.Parameters)
                    {
                        var dbParam = cmd.CreateParameter();
                        spParam.ApplyTo(dbParam);
                        cmd.Parameters.Add(dbParam);
                        paramPairs.Add(Tuple.Create(spParam, dbParam));
                    }

                    if (parameters.ReturnParameter != null)
                    {
                        var dbParam = cmd.CreateParameter();
                        dbParam.Direction = parameters.ReturnParameter.Direction;
                        dbParam.DbType = parameters.ReturnParameter.DbType;
                        dbParam.ParameterName = parameters.ReturnParameter.Name;
                        cmd.Parameters.Add(dbParam);
                        paramPairs.Add(Tuple.Create(parameters.ReturnParameter, dbParam));
                    }
                }

                cmd.ExecuteNonQuery();

                if (paramPairs.Count > 0)
                {
                    for (int i = 0; i < paramPairs.Count; i++)
                    {
                        var pair = paramPairs[i];
                        var spParam = pair.Item1;
                        var dbParam = pair.Item2;
                        if (dbParam.Direction == ParameterDirection.Output
                            || dbParam.Direction == ParameterDirection.ReturnValue
                            || dbParam.Direction == ParameterDirection.InputOutput)
                        {
                            spParam.Value = dbParam.Value;
                        }
                    }
                }
            }
        }
    }
}
