namespace Blazer
{
    using System.Linq.Expressions;
    using System.Reflection;

    internal interface IDataTypeMapper
    {
        Expression GetReaderExpression(Expression entityFieldExpr, ParameterExpression dataRecordParameterExpr, MethodInfo dataRecordIsDbNullMethod, int fieldIndex);
    }
}
