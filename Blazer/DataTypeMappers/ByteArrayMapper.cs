namespace Blazer.DataTypeMappers
{
    using System.Data;
    using System.Linq.Expressions;
    using System.Reflection;

    /// <summary>
    /// Data Mapper for dealing with binary arrays (e.g. varbinary(max) columns).
    /// Does not support content larger than 2^31 - 1 bytes in size.
    /// </summary>
    internal class ByteArrayMapper : IDataTypeMapper
    {
        public Expression GetReaderExpression(
            Expression entityFieldExpr,
            ParameterExpression dataRecordParameterExpr,
            MethodInfo dataRecordIsDbNullMethod,
            int fieldIndex)
        {
            var getBytesMethod = typeof(IDataRecord).GetMethod("GetBytes", new[] { typeof(int), typeof(long), typeof(byte[]), typeof(int), typeof(int) });

            // int length;
            var lengthVarExpr = Expression.Variable(typeof(int), "length");

            // length = (int)<data_record>.GetBytes(fieldIndex, 0, null, 0, 0);
            var lengthAssignExpr = Expression.Assign(
                lengthVarExpr,
                Expression.Convert(
                    Expression.Call(
                        dataRecordParameterExpr,
                        getBytesMethod,
                        Expression.Constant(fieldIndex),
                        Expression.Constant(0L),
                        Expression.Constant(null, typeof(byte[])),
                        Expression.Constant(0),
                        Expression.Constant(0)),
                    typeof(int)));

            // entity.<some_field> = new byte[length];
            var newByteArrayAssignExpr = Expression.Assign(
                entityFieldExpr,
                Expression.NewArrayBounds(typeof(byte), lengthVarExpr));

            // <data_record>.GetBytes(fieldIndex, 0, entity.<some_field>, 0, length);
            var readBytesExpr = Expression.Call(
                dataRecordParameterExpr,
                getBytesMethod,
                Expression.Constant(fieldIndex),
                Expression.Constant(0L),
                entityFieldExpr,
                Expression.Constant(0),
                lengthVarExpr);

            var readArrayBlockExpr = Expression.Block(
                new[] { lengthVarExpr },
                lengthAssignExpr,
                newByteArrayAssignExpr,
                readBytesExpr);

            // if (!record.IsDbNull(fieldIndex))
            // {
            //      ...
            // }
            var ifNullExpr = Expression.IfThen(
                Expression.Not(
                    Expression.Call(
                        dataRecordParameterExpr,
                        dataRecordIsDbNullMethod,
                        Expression.Constant(fieldIndex))),
                readArrayBlockExpr);

            return ifNullExpr;
        }
    }
}
