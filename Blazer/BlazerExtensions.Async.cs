#if FEATURE_ASYNC
namespace Blazer
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.Common;
    using System.Threading.Tasks;

    public static partial class BlazerExtensions
    {
#if FEATURE_FORMATTABLE_STRING
        public static async Task<T> ScalarAsync<T>(this IDbConnection connection, FormattableString command, CommandConfiguration config = null)
        {
            using (var cmd = (DbCommand)CommandFactory.CreateCommand(connection, command.Format, config))
            {
                var cancelToken = CommandFactory.GetCancellationToken(config);

                if (command.ArgumentCount > 0)
                {
                    ParameterFactory.AddParameters(cmd, command);
                }

                var value = await cmd.ExecuteScalarAsync(cancelToken).ConfigureAwait(false);
                if (value == null || value == DBNull.Value)
                {
                    return default(T);
                }
                return (T)value;
            }
        }

        public static async Task<T> ScalarAsync<T>(this IDbConnection connection, NonFormattableString command, object parameters = null, CommandConfiguration config = null)
        {
            using (var cmd = (DbCommand)CommandFactory.CreateCommand(connection, command.Value, config))
            {
#else
        public static async Task<T> ScalarAsync<T>(this IDbConnection connection, string command, object parameters = null, CommandConfiguration config = null)
        {
            using (var cmd = (DbCommand)CommandFactory.CreateCommand(connection, command, config))
            {
#endif
                var cancelToken = CommandFactory.GetCancellationToken(config);

                if (parameters != null)
                {
                    ParameterFactory.AddParameters(cmd, parameters);
                }

                var value = await cmd.ExecuteScalarAsync(cancelToken).ConfigureAwait(false);
                if (value == null || value == DBNull.Value)
                {
                    return default(T);
                }
                return (T)value;
            }
        }

#if FEATURE_FORMATTABLE_STRING
        public static async Task<int> CommandAsync(this IDbConnection connection, FormattableString command, CommandConfiguration config = null)
        {
            using (var cmd = (DbCommand)CommandFactory.CreateCommand(connection, command.Format, config))
            {
                var cancelToken = CommandFactory.GetCancellationToken(config);

                if (command.ArgumentCount > 0)
                {
                    ParameterFactory.AddParameters(cmd, command);
                }

                return await cmd.ExecuteNonQueryAsync(cancelToken).ConfigureAwait(false);
            }
        }

        public static async Task<int> CommandAsync(this IDbConnection connection, NonFormattableString command, object parameters = null, CommandConfiguration config = null)
        {
            using (var cmd = (DbCommand)CommandFactory.CreateCommand(connection, command.Value, config))
            {
#else
        public static async Task<int> CommandAsync(this IDbConnection connection, string command, object parameters = null, CommandConfiguration config = null)
        {
            using (var cmd = (DbCommand)CommandFactory.CreateCommand(connection, command, config))
            {
#endif
                var cancelToken = CommandFactory.GetCancellationToken(config);

                if (parameters != null)
                {
                    ParameterFactory.AddParameters(cmd, parameters);
                }

                return await cmd.ExecuteNonQueryAsync(cancelToken).ConfigureAwait(false);
            }
        }

#if FEATURE_FORMATTABLE_STRING
        public static async Task<IEnumerable<T>> QueryAsync<T>(this IDbConnection connection, FormattableString command, CommandConfiguration config = null) where T : new()
        {
            using (var cmd = (DbCommand)CommandFactory.CreateCommand(connection, command.Format, config))
            {
                var cancelToken = CommandFactory.GetCancellationToken(config);

                if (command.ArgumentCount > 0)
                {
                    ParameterFactory.AddParameters(cmd, command);
                }

                using (var reader = await cmd.ExecuteReaderAsync(s_defaultCommandBehavior, cancelToken).ConfigureAwait(false))
                {
                    var results = new List<T>();
                    var mapper = DataMapperFactory.GetMapper<T>(cmd, reader);
                    while (await reader.ReadAsync(cancelToken).ConfigureAwait(false))
                    {
                        results.Add((T)mapper(reader));
                    }
                    return results;
                }
            }
        }

        public static async Task<IEnumerable<T>> QueryAsync<T>(this IDbConnection connection, NonFormattableString command, object parameters = null, CommandConfiguration config = null) where T : new()
        {
            using (var cmd = (DbCommand)CommandFactory.CreateCommand(connection, command.Value, config))
            {
#else
        public static async Task<IEnumerable<T>> QueryAsync<T>(this IDbConnection connection, string command, object parameters = null, CommandConfiguration config = null) where T : new()
        {
            using (var cmd = (DbCommand)CommandFactory.CreateCommand(connection, command, config))
            {
#endif
                var cancelToken = CommandFactory.GetCancellationToken(config);

                if (parameters != null)
                {
                    ParameterFactory.AddParameters(cmd, parameters);
                }

                using (var reader = await cmd.ExecuteReaderAsync(s_defaultCommandBehavior, cancelToken).ConfigureAwait(false))
                {
                    var results = new List<T>();
                    var mapper = DataMapperFactory.GetMapper<T>(cmd, reader);
                    while (await reader.ReadAsync(cancelToken).ConfigureAwait(false))
                    {
                        results.Add((T)mapper(reader));
                    }
                    return results;
                }
            }
        }

#if FEATURE_FORMATTABLE_STRING
        public static async Task<T> QuerySingleAsync<T>(this IDbConnection connection, FormattableString command, CommandConfiguration config = null) where T : new()
        {
            using (var cmd = (DbCommand)CommandFactory.CreateCommand(connection, command.Format, config))
            {
                var cancelToken = CommandFactory.GetCancellationToken(config);

                if (command.ArgumentCount > 0)
                {
                    ParameterFactory.AddParameters(cmd, command);
                }

                using (var reader = await cmd.ExecuteReaderAsync(s_defaultCommandBehavior | CommandBehavior.SingleRow, cancelToken).ConfigureAwait(false))
                {
                    var mapper = DataMapperFactory.GetMapper<T>(cmd, reader);
                    if (await reader.ReadAsync(cancelToken).ConfigureAwait(false))
                    {
                        return (T)mapper(reader);
                    }
                    return default(T);
                }
            }
        }

        public static async Task<T> QuerySingleAsync<T>(this IDbConnection connection, NonFormattableString command, object parameters = null, CommandConfiguration config = null) where T : new()
        {
            using (var cmd = (DbCommand)CommandFactory.CreateCommand(connection, command.Value, config))
            {
#else
        public static async Task<T> QuerySingleAsync<T>(this IDbConnection connection, string command, object parameters = null, CommandConfiguration config = null) where T : new()
        {
            using (var cmd = (DbCommand)CommandFactory.CreateCommand(connection, command, config))
            {
#endif
                var cancelToken = CommandFactory.GetCancellationToken(config);

                if (parameters != null)
                {
                    ParameterFactory.AddParameters(cmd, parameters);
                }

                using (var reader = await cmd.ExecuteReaderAsync(s_defaultCommandBehavior | CommandBehavior.SingleRow, cancelToken).ConfigureAwait(false))
                {
                    var mapper = DataMapperFactory.GetMapper<T>(cmd, reader);
                    if (await reader.ReadAsync(cancelToken).ConfigureAwait(false))
                    {
                        return (T)mapper(reader);
                    }
                    return default(T);
                }
            }
        }

#if FEATURE_FORMATTABLE_STRING
        public static async Task<IEnumerable<dynamic>> QueryAsync(this IDbConnection connection, FormattableString command, CommandConfiguration config = null)
        {
            using (var cmd = (DbCommand)CommandFactory.CreateCommand(connection, command.Format, config))
            {
                var cancelToken = CommandFactory.GetCancellationToken(config);

                if (command.ArgumentCount > 0)
                {
                    ParameterFactory.AddParameters(cmd, command);
                }

                using (var reader = await cmd.ExecuteReaderAsync(s_defaultCommandBehavior, cancelToken).ConfigureAwait(false))
                {
                    var results = new List<dynamic>();
                    var mapper = DataMapperFactory.GetMapper(cmd, reader);
                    while (await reader.ReadAsync(cancelToken).ConfigureAwait(false))
                    {
                        results.Add(mapper(reader));
                    }
                    return results;
                }
            }
        }

        public static async Task<IEnumerable<dynamic>> QueryAsync(this IDbConnection connection, NonFormattableString command, object parameters = null, CommandConfiguration config = null)
        {
            using (var cmd = (DbCommand)CommandFactory.CreateCommand(connection, command.Value, config))
            {
#else
        public static async Task<IEnumerable<dynamic>> QueryAsync(this IDbConnection connection, string command, object parameters = null, CommandConfiguration config = null)
        {
            using (var cmd = (DbCommand)CommandFactory.CreateCommand(connection, command, config))
            {
#endif
                var cancelToken = CommandFactory.GetCancellationToken(config);

                if (parameters != null)
                {
                    ParameterFactory.AddParameters(cmd, parameters);
                }

                using (var reader = await cmd.ExecuteReaderAsync(s_defaultCommandBehavior, cancelToken).ConfigureAwait(false))
                {
                    var results = new List<dynamic>();
                    var mapper = DataMapperFactory.GetMapper(cmd, reader);
                    while (await reader.ReadAsync(cancelToken).ConfigureAwait(false))
                    {
                        results.Add(mapper(reader));
                    }
                    return results;
                }
            }
        }

#if FEATURE_FORMATTABLE_STRING
        public static async Task<dynamic> QuerySingleAsync(this IDbConnection connection, FormattableString command, CommandConfiguration config = null)
        {
            using (var cmd = (DbCommand)CommandFactory.CreateCommand(connection, command.Format, config))
            {
                var cancelToken = CommandFactory.GetCancellationToken(config);

                if (command.ArgumentCount > 0)
                {
                    ParameterFactory.AddParameters(cmd, command);
                }

                using (var reader = await cmd.ExecuteReaderAsync(s_defaultCommandBehavior | CommandBehavior.SingleRow, cancelToken).ConfigureAwait(false))
                {
                    var mapper = DataMapperFactory.GetMapper(cmd, reader);
                    if (await reader.ReadAsync(cancelToken).ConfigureAwait(false))
                    {
                        return mapper(reader);
                    }
                    return null;
                }
            }
        }

        public static async Task<dynamic> QuerySingleAsync(this IDbConnection connection, NonFormattableString command, object parameters = null, CommandConfiguration config = null)
        {
            using (var cmd = (DbCommand)CommandFactory.CreateCommand(connection, command.Value, config))
            {
#else
        public static async Task<dynamic> QuerySingleAsync(this IDbConnection connection, string command, object parameters = null, CommandConfiguration config = null)
        {
            using (var cmd = (DbCommand)CommandFactory.CreateCommand(connection, command, config))
            {
#endif
                var cancelToken = CommandFactory.GetCancellationToken(config);

                if (parameters != null)
                {
                    ParameterFactory.AddParameters(cmd, parameters);
                }

                using (var reader = await cmd.ExecuteReaderAsync(s_defaultCommandBehavior | CommandBehavior.SingleRow, cancelToken).ConfigureAwait(false))
                {
                    var mapper = DataMapperFactory.GetMapper(cmd, reader);
                    if (await reader.ReadAsync(cancelToken).ConfigureAwait(false))
                    {
                        return mapper(reader);
                    }
                    return null;
                }
            }
        }

        public static async Task StoredProcedureAsync(this IDbConnection connection, string command, SpParameters parameters = null, CommandConfiguration config = null)
        {
            using (var cmd = (DbCommand)CommandFactory.CreateCommand(connection, command, config))
            {
                var cancelToken = CommandFactory.GetCancellationToken(config);

                cmd.CommandType = CommandType.StoredProcedure;

                var paramPairs = new List<Tuple<SpParameter, DbParameter>>();
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

                await cmd.ExecuteNonQueryAsync(cancelToken);

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
#endif
