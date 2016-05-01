namespace Blazer
{
    using System.Data;

    internal static class CommandFactory
    {
        public static IDbCommand CreateCommand(IDbConnection connection, string commandText, CommandConfiguration commandConfig)
        {
            var defaultConfig = CommandConfiguration.Default.Copy();
            var missingOptionBehaviour = CommandConfiguration.OnUnsetConfigurationOption;

            var command = connection.CreateCommand();
            command.CommandText = commandText;

            if (commandConfig != null)
            {
                if (commandConfig.Timeout.HasValue)
                {
                    command.CommandTimeout = commandConfig.Timeout.Value;
                }
                else if (missingOptionBehaviour == UnsetConfigurationOptionBehaviour.UseDefault
                    && defaultConfig.Timeout.HasValue)
                {
                    command.CommandTimeout = defaultConfig.Timeout.Value;
                }

                if (commandConfig.CommandType.HasValue)
                {
                    command.CommandType = commandConfig.CommandType.Value;
                }
                else if (missingOptionBehaviour == UnsetConfigurationOptionBehaviour.UseDefault
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
    }
}
