namespace Blazer
{
    using System;
    using System.Collections.Generic;
    using System.Data;

    public class SpParameters
    {
        readonly Dictionary<string, SpParameter> m_params = new Dictionary<string, SpParameter>();
        SpParameter m_return = null;

        public void AddInput(string name, object value, int? size = null, byte? precision = null, byte? scale = null)
        {
            DbType dbType;
            if (DbTypeStore.TryGetDbType(value.GetType(), out dbType))
            {
                m_params.Add(name, GetSpParam(name, dbType, ParameterDirection.Input, value, size, precision, scale));
            }
            else
            {
                throw new NotSupportedException($"Parameter of type {value.GetType()} is not supported.");
            }
        }

        public void AddOutput(string name, DbType dbType, int? size = null, byte? precision = null, byte? scale = null)
        {
            m_params.Add(name, GetSpParam(name, dbType, ParameterDirection.Output, null, size, precision, scale));
        }

        public void AddInputOutput(string name, object value, int? size = null, byte? precision = null, byte? scale = null)
        {
            DbType dbType;
            if (DbTypeStore.TryGetDbType(value.GetType(), out dbType))
            {
                m_params.Add(name, GetSpParam(name, dbType, ParameterDirection.InputOutput, value, size, precision, scale));
            }
            else
            {
                throw new NotSupportedException($"Parameter of type {value.GetType()} is not supported.");
            }
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
                return (T)spParam.Value;
            }

            throw new KeyNotFoundException($"Output parameter not found: {name}.");
        }

        public T GetReturnValue<T>()
        {
            if (m_return != null)
            {
                return (T)m_return.Value;
            }

            throw new InvalidOperationException("No return parameter was provided.");
        }
    }
}
