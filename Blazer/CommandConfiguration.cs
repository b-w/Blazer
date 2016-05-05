namespace Blazer
{
    using System.Data;
    using System.Threading;

    public class DefaultCommandConfiguration
    {
        readonly ReaderWriterLockSlim m_lock = new ReaderWriterLockSlim();
        int? m_timeout;
        CommandType? m_commandType;

        internal DefaultCommandConfiguration()
        {
        }

        public int? Timeout
        {
            get
            {
                return m_timeout;
            }
            set
            {
                m_lock.EnterWriteLock();
                try
                {
                    m_timeout = value;
                }
                finally
                {
                    m_lock.ExitWriteLock();
                }
            }
        }

        public CommandType? CommandType
        {
            get
            {
                return m_commandType;
            }
            set
            {
                m_lock.EnterWriteLock();
                try
                {
                    m_commandType = value;
                }
                finally
                {
                    m_lock.ExitWriteLock();
                }
            }
        }

        internal DefaultCommandConfiguration Copy()
        {
            m_lock.EnterReadLock();
            try
            {
                return new DefaultCommandConfiguration()
                {
                    m_timeout = this.m_timeout,
                    m_commandType = this.m_commandType
                };
            }
            finally
            {
                m_lock.ExitReadLock();
            }
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
