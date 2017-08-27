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
            dbParameter.Direction = Direction;
            dbParameter.DbType = DbType;
            dbParameter.ParameterName = Name;

            if (dbParameter.Direction == ParameterDirection.Input
                || dbParameter.Direction == ParameterDirection.InputOutput)
            {
                dbParameter.Value = Value ?? DBNull.Value;
            }

            if (Size.HasValue)
            {
                dbParameter.Size = Size.Value;
            }

            if (Precision.HasValue)
            {
                dbParameter.Precision = Precision.Value;
            }

            if (Scale.HasValue)
            {
                dbParameter.Scale = Scale.Value;
            }
        }
    }
}
