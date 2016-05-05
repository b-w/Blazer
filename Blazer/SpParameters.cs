namespace Blazer
{
    using System;
    using System.Collections.Generic;
    using System.Data;

    public class SpParameters
    {
        readonly Dictionary<string, SpParameter> m_params = new Dictionary<string, SpParameter>();
        SpParameter m_return = null;

        public void AddInput(string name, object value, DbType? dbType = null, int? size = null, byte? precision = null, byte? scale = null)
        {
            DbType dbTypeValue;
            if (dbType.HasValue)
            {
                dbTypeValue = dbType.Value;
            }
            else if (value == null)
            {
                throw new ArgumentException($"Argument '{nameof(dbType)}' cannot be null if '{nameof(value)}' is null.");
            }
            else if (!DbTypeStore.TryGetDbType(value.GetType(), out dbTypeValue))
            {
                throw new NotSupportedException($"Parameter of type {value.GetType()} is not supported.");
            }
            m_params.Add(name, GetSpParam(name, dbTypeValue, ParameterDirection.Input, value, size, precision, scale));
        }

        public void AddOutput(string name, DbType dbType, int? size = null, byte? precision = null, byte? scale = null)
        {
            m_params.Add(name, GetSpParam(name, dbType, ParameterDirection.Output, null, size, precision, scale));
        }

        public void AddInputOutput(string name, object value, DbType? dbType = null, int? size = null, byte? precision = null, byte? scale = null)
        {
            DbType dbTypeValue;
            if (dbType.HasValue)
            {
                dbTypeValue = dbType.Value;
            }
            else if (value == null)
            {
                throw new ArgumentException($"Argument '{nameof(dbType)}' cannot be null if '{nameof(value)}' is null.");
            }
            else if (!DbTypeStore.TryGetDbType(value.GetType(), out dbTypeValue))
            {
                throw new NotSupportedException($"Parameter of type {value.GetType()} is not supported.");
            }
            m_params.Add(name, GetSpParam(name, dbTypeValue, ParameterDirection.InputOutput, value, size, precision, scale));
        }

        public void SetReturn(string name, DbType dbType, int? size = null, byte? precision = null, byte? scale = null)
        {
            m_return = GetSpParam(name, dbType, ParameterDirection.ReturnValue, null, size, precision, scale);
        }

        static SpParameter GetSpParam(string name, DbType dbType, ParameterDirection direction, object value, int? size, byte? precision, byte? scale)
        {
            var spParam = new SpParameter()
            {
                Name = name,
                DbType = dbType,
                Direction = direction,
                Value = value
            };
            if (size.HasValue)
            {
                spParam.Size = size.Value;
            }
            if (precision.HasValue)
            {
                spParam.Precision = precision.Value;
            }
            if (scale.HasValue)
            {
                spParam.Scale = scale.Value;
            }
            return spParam;
        }

        internal IReadOnlyCollection<SpParameter> Parameters
        {
            get { return m_params.Values; }
        }

        internal SpParameter ReturnParameter
        {
            get { return m_return; }
        }

        public T GetOutputValue<T>(string name)
        {
            SpParameter spParam;
            if (m_params.TryGetValue(name, out spParam))
            {
                var outputValue = spParam.Value;
                if (outputValue == DBNull.Value)
                {
                    return default(T);
                }
                return (T)outputValue;
            }

            throw new KeyNotFoundException($"Output parameter not found: {name}.");
        }

        public T GetReturnValue<T>()
        {
            if (m_return != null)
            {
                var returnValue = m_return.Value;
                if (returnValue == DBNull.Value)
                {
                    return default(T);
                }
                return (T)returnValue;
            }

            throw new InvalidOperationException("No return parameter was provided.");
        }
    }
}
