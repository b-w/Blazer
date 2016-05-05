namespace Blazer
{
    using System;
    using System.Data;

    internal sealed class SpParameter
    {
        public string Name { get; set; }

        public DbType DbType { get; set; }

        public ParameterDirection Direction { get; set; }

        public object Value { get; set; }

        public int? Size { get; set; }

        public byte? Precision { get; set; }

        public byte? Scale { get; set; }

        public void ApplyTo(IDbDataParameter dbParameter)
        {
            dbParameter.Direction = this.Direction;
            dbParameter.DbType = this.DbType;
            dbParameter.ParameterName = this.Name;
            if (dbParameter.Direction == ParameterDirection.Input
                || dbParameter.Direction == ParameterDirection.InputOutput)
            {
                dbParameter.Value = this.Value ?? DBNull.Value;
            }
            if (this.Size.HasValue)
            {
                dbParameter.Size = this.Size.Value;
            }
            if (this.Precision.HasValue)
            {
                dbParameter.Precision = this.Precision.Value;
            }
            if (this.Scale.HasValue)
            {
                dbParameter.Scale = this.Scale.Value;
            }
        }
    }
}
