namespace Blazer
{
    using System.Data;
    using System.Threading;

    public class DefaultCommandConfiguration
    {
        internal DefaultCommandConfiguration()
        {
        }

        public int? Timeout { get; set; }

        public CommandType? CommandType { get; set; }

        internal DefaultCommandConfiguration Copy()
        {
            return new DefaultCommandConfiguration()
            {
                Timeout = this.Timeout,
                CommandType = this.CommandType
            };
        }
    }

    public enum UnsetConfigurationOptionBehavior
    {
        Ignore = 0,
        UseDefault
    }

    public class CommandConfiguration : DefaultCommandConfiguration
    {
        static CommandConfiguration()
        {
            Default = new DefaultCommandConfiguration();
            OnUnsetConfigurationOption = UnsetConfigurationOptionBehavior.UseDefault;
        }

        public static DefaultCommandConfiguration Default { get; protected set; }

        public static UnsetConfigurationOptionBehavior OnUnsetConfigurationOption { get; set; }

        public IDbTransaction Transaction { get; set; }

        public CancellationToken? AsyncCancellationToken { get; set; }
    }
}
