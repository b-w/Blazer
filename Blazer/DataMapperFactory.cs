namespace Blazer
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data;
    using System.Linq.Expressions;
    using System.Reflection;

    internal static class DataMapperFactory
    {
        const BindingFlags FLAGS_ALLINST = BindingFlags.Instance | BindingFlags.Public;

        public delegate object DataMapper(IDataRecord record);

        public static DataMapper GetMapper<T>(IDataReader reader) where T : new()
        {
            var entityType = typeof(T);
            var dataRecordType = typeof(IDataRecord);
            var dataRecordParamExpr = Expression.Parameter(dataRecordType, "record");
            var dataRecordIsDbNullMethod = dataRecordType.GetMethod("IsDBNull", new[] { typeof(int) });

            var mapperBodyExpressions = new List<Expression>();
            var returnLabelExpr = Expression.Label(entityType);

            // T entity;
            var entityVarExpr = Expression.Variable(entityType, "entity");

            // entity = new T();
            var entityNewExpr = Expression.Assign(
                entityVarExpr,
                Expression.New(entityType));
            mapperBodyExpressions.Add(entityNewExpr);

            for (int i = 0; i < reader.FieldCount; i++)
            {
                var entityField = GetEntityField(entityType, reader.GetName(i));
                if (entityField == null)
                {
                    continue;
                }

                Type entityFieldType = null;
                Expression entityFieldExpr = null;

                if (entityField is PropertyInfo)
                {
                    var propInfo = (PropertyInfo)entityField;
                    entityFieldType = propInfo.PropertyType;
                    entityFieldExpr = Expression.Property(entityVarExpr, propInfo);
                }
                else if (entityField is FieldInfo)
                {
                    var fieldInfo = (FieldInfo)entityField;
                    entityFieldType = fieldInfo.FieldType;
                    entityFieldExpr = Expression.Field(entityVarExpr, fieldInfo);
                }

                var readExpr = GetReaderExpression(entityFieldType, dataRecordParamExpr, i);

                // entity.<some_field> = <reader_expr>;
                Expression entitySetFieldExpr = Expression.Assign(
                    entityFieldExpr,
                    readExpr);

                if (Nullable.GetUnderlyingType(entityFieldType) != null || !entityFieldType.IsValueType)
                {
                    // if (!record.IsDbNull(i))
                    // {
                    //      entity.<some_field> = <reader_expr>;
                    // }
                    entitySetFieldExpr = Expression.IfThen(
                        Expression.Not(
                            Expression.Call(
                                dataRecordParamExpr,
                                dataRecordIsDbNullMethod,
                                Expression.Constant(i))),
                        entitySetFieldExpr);
                }

                mapperBodyExpressions.Add(entitySetFieldExpr);
            }

            // return entity;
            var entityReturnExpr = Expression.Return(returnLabelExpr, entityVarExpr);
            mapperBodyExpressions.Add(entityReturnExpr);

            // default return value
            var defaultReturnExpr = Expression.Label(returnLabelExpr, Expression.Constant(null, entityType));
            mapperBodyExpressions.Add(defaultReturnExpr);

            var lambdaBodyExpr = Expression.Block(
                new[] { entityVarExpr },
                mapperBodyExpressions);
            var lambdaExpr = Expression.Lambda<DataMapper>(lambdaBodyExpr, dataRecordParamExpr);
            return lambdaExpr.Compile();
        }

        static MemberInfo GetEntityField(Type entityType, string columnName)
        {
            var members = new List<MemberInfo>();
            members.AddRange(entityType.GetProperties(FLAGS_ALLINST));
            members.AddRange(entityType.GetFields(FLAGS_ALLINST));

            foreach (var member in members)
            {
                var columnAttr = member.GetCustomAttribute<ColumnAttribute>();
                if (columnAttr != null && String.Equals(columnAttr.Name, columnName, StringComparison.OrdinalIgnoreCase))
                {
                    return member;
                }
            }

            foreach (var member in members)
            {
                if (String.Equals(member.Name, columnName, StringComparison.OrdinalIgnoreCase))
                {
                    return member;
                }
            }

            return null;
        }

        static Expression GetReaderExpression(Type fieldType, ParameterExpression dataRecordParameterExpr, int fieldIndex)
        {
            MethodInfo method;
            if (DataRecordMap.TryGetGetMethod(fieldType, out method))
            {
                Expression callReaderExpr = Expression.Call(dataRecordParameterExpr, method, Expression.Constant(fieldIndex));
                if (fieldType.IsEnum || Nullable.GetUnderlyingType(fieldType) != null)
                {
                    callReaderExpr = Expression.Convert(callReaderExpr, fieldType);
                }
                return callReaderExpr;
            }
            throw new NotSupportedException($"Field of type {fieldType} is not supported.");
        }
    }
}
