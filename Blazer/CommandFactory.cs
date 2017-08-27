namespace Blazer
{
    using System.Data;
    using System.Threading;

    internal static class CommandFactory
    {
        public static IDbCommand CreateCommand(IDbConnection connection, string commandText, CommandConfiguration commandConfig)
        {
            var defaultConfig = CommandConfiguration.Default.Copy();
            var missingOptionBehavior = CommandConfiguration.OnUnsetConfigurationOption;

            var command = connection.CreateCommand();
            command.CommandText = commandText;

            if (commandConfig != null)
            {
                if (commandConfig.Timeout.HasValue)
                {
                    command.CommandTimeout = commandConfig.Timeout.Value;
                }
                else if (missingOptionBehavior == UnsetConfigurationOptionBehavior.UseDefault
                    && defaultConfig.Timeout.HasValue)
                {
                    command.CommandTimeout = defaultConfig.Timeout.Value;
                }

                if (commandConfig.CommandType.HasValue)
                {
                    command.CommandType = commandConfig.CommandType.Value;
                }
                else if (missingOptionBehavior == UnsetConfigurationOptionBehavior.UseDefault
                    && defaultConfig.CommandType.HasValue)
                {
                    command.CommandType = defaultConfig.CommandType.Value;
                }

                if (commandConfig.Transaction != null)
                {
                    command.Transaction = commandConfig.Transaction;
                }
            }
            else
            {
                if (defaultConfig.Timeout.HasValue)
                {
                    command.CommandTimeout = defaultConfig.Timeout.Value;
                }

                if (defaultConfig.CommandType.HasValue)
                {
                    command.CommandType = defaultConfig.CommandType.Value;
                }
            }

            return command;
        }

        public static CancellationToken GetCancellationToken(CommandConfiguration commandConfig)
        {
            if (commandConfig?.AsyncCancellationToken.HasValue == true)
            {
                return commandConfig.AsyncCancellationToken.Value;
            }

            return CancellationToken.None;
        }
    }
}
