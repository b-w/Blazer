namespace Blazer
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data;
    using System.Dynamic;
    using System.Linq.Expressions;
    using System.Reflection;
    using Blazer.Caching;

    internal static class DataMapperFactory
    {
        class Context
        {
            public Type EntityFieldType { get; set; }
            public Expression EntityFieldExpr { get; set; }
            public ParameterExpression DataRecordParameterExpr { get; set; }
            public MethodInfo DataRecordIsDbNullMethod { get; set; }
            public int FieldIndex { get; set; }
        }

        const BindingFlags FLAGS_ALLINST = BindingFlags.Instance | BindingFlags.Public;

        public delegate object DataMapper(IDataRecord record);

        public static DataMapper GetMapper(IDataReader reader)
        {
            var columns = new string[reader.FieldCount];
            for (int i = 0; i < reader.FieldCount; i++)
            {
                columns[i] = reader.GetName(i);
            }

            return (record) =>
            {
                dynamic expando = new ExpandoObject();
                var expandoDict = (IDictionary<string, object>)expando;
                var values = new object[record.FieldCount];
                record.GetValues(values);
                for (int i = 0; i < record.FieldCount; i++)
                {
                    var value = values[i];
                    expandoDict[columns[i]] = (value == DBNull.Value ? null : value);
                }
                return expando;
            };
        }

        public static DataMapper GetMapper<T>(IDbCommand command, IDataReader reader) where T : new()
        {
            var entityType = typeof(T);

            var cacheKey = new DataMapperCache.Key(command, entityType);
            DataMapper cachedMapper;
            if (DataMapperCache.TryGet(cacheKey, out cachedMapper))
            {
                return cachedMapper;
            }

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

                var ctx = new Context()
                {
                    EntityFieldType = entityFieldType,
                    EntityFieldExpr = entityFieldExpr,
                    DataRecordParameterExpr = dataRecordParamExpr,
                    DataRecordIsDbNullMethod = dataRecordIsDbNullMethod,
                    FieldIndex = i
                };

                mapperBodyExpressions.Add(GetFieldMapperExpression(ctx));
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
            var lambda = lambdaExpr.Compile();

            DataMapperCache.Add(cacheKey, lambda);

            return lambda;
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

        static Expression GetFieldMapperExpression(Context context)
        {
            IDataTypeMapper mapper;
            if (DataTypeMapperStore.TryGetMapper(context.EntityFieldType, out mapper))
            {
                return mapper.GetReaderExpression(
                    context.EntityFieldExpr,
                    context.DataRecordParameterExpr,
                    context.DataRecordIsDbNullMethod,
                    context.FieldIndex);
            }
            return GetValueTypeFieldMapperExpression(context);
        }

        static Expression GetValueTypeFieldMapperExpression(Context context)
        {
            var readExpr = GetValueTypeReaderExpression(context);

            // entity.<some_field> = <reader_expr>;
            Expression entitySetFieldExpr = Expression.Assign(
                context.EntityFieldExpr,
                readExpr);

            if (Nullable.GetUnderlyingType(context.EntityFieldType) != null || !context.EntityFieldType.IsValueType)
            {
                // if (!record.IsDbNull(i))
                // {
                //      entity.<some_field> = <reader_expr>;
                // }
                entitySetFieldExpr = Expression.IfThen(
                    Expression.Not(
                        Expression.Call(
                            context.DataRecordParameterExpr,
                            context.DataRecordIsDbNullMethod,
                            Expression.Constant(context.FieldIndex))),
                    entitySetFieldExpr);
            }
            return entitySetFieldExpr;
        }

        static Expression GetValueTypeReaderExpression(Context context)
        {
            MethodInfo method;
            if (DataRecordMap.TryGetGetMethod(context.EntityFieldType, out method))
            {
                Expression callReaderExpr = Expression.Call(context.DataRecordParameterExpr, method, Expression.Constant(context.FieldIndex));
                if (context.EntityFieldType.IsEnum || Nullable.GetUnderlyingType(context.EntityFieldType) != null)
                {
                    callReaderExpr = Expression.Convert(callReaderExpr, context.EntityFieldType);
                }
                return callReaderExpr;
            }
            throw new NotSupportedException($"Field of type {context.EntityFieldType} is not supported.");
        }
    }
}
