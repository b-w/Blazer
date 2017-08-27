namespace Blazer
{
    using System;
    using System.Collections.Generic;
#if FEATURE_DATA_ANNOTATIONS
    using System.ComponentModel.DataAnnotations.Schema;
#endif
    using System.Data;
    using System.Linq.Expressions;
    using System.Reflection;

    using Blazer.Caching;
    using Blazer.Dynamic;

    internal static class DataMapperFactory
    {
        private class Context
        {
            public Type EntityFieldType { get; set; }

            public Expression EntityFieldExpr { get; set; }

            public ParameterExpression DataRecordParameterExpr { get; set; }

            public MethodInfo DataRecordIsDbNullMethod { get; set; }

            public int FieldIndex { get; set; }
        }

        private const BindingFlags FLAGS_PUBINST = BindingFlags.Instance | BindingFlags.Public;

        public delegate object DataMapper(IDataRecord record);

        public static DataMapper GetMapper(IDbCommand command, IDataReader reader)
        {
            var cacheKey = new DataMapperCache.Key(command, typeof(BlazerDynamicObject));

            if (DataMapperCache.TryGet(cacheKey, out DataMapper cachedMapper))
            {
                return cachedMapper;
            }

            var columns = new string[reader.FieldCount];
            for (int i = 0; i < reader.FieldCount; i++)
            {
                columns[i] = reader.GetName(i);
            }

            DataMapper lambda = (record) =>
            {
                var values = new object[record.FieldCount];
                var blzDynamic = new BlazerDynamicObject(values.Length);

                record.GetValues(values);

                for (int i = 0; i < values.Length; i++)
                {
                    var value = values[i];
                    blzDynamic.Set(columns[i], value == DBNull.Value ? null : value);
                }

                return blzDynamic;
            };

            DataMapperCache.Add(cacheKey, lambda);

            return lambda;
        }

        public static DataMapper GetMapper<T>(IDbCommand command, IDataReader reader) where T : new()
        {
            var entityType = typeof(T);
            var cacheKey = new DataMapperCache.Key(command, entityType);

            if (DataMapperCache.TryGet(cacheKey, out DataMapper cachedMapper))
            {
                return cachedMapper;
            }

            var dataRecordType = typeof(IDataRecord);
            var dataRecordParamExpr = Expression.Parameter(dataRecordType, "record");
            var dataRecordIsDbNullMethod = dataRecordType.GetMethod(nameof(IDataRecord.IsDBNull), new[] { typeof(int) });

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

                if (entityField is PropertyInfo propInfo)
                {
                    entityFieldType = propInfo.PropertyType;
                    entityFieldExpr = Expression.Property(entityVarExpr, propInfo);
                }
                else if (entityField is FieldInfo fieldInfo)
                {
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

        private static MemberInfo GetEntityField(Type entityType, string columnName)
        {
            var members = new List<MemberInfo>();
            members.AddRange(entityType.GetProperties(FLAGS_PUBINST));
            members.AddRange(entityType.GetFields(FLAGS_PUBINST));

#if FEATURE_DATA_ANNOTATIONS
            foreach (var member in members)
            {
                var columnAttr = member.GetCustomAttribute<ColumnAttribute>();
                if (columnAttr != null && String.Equals(columnAttr.Name, columnName, StringComparison.OrdinalIgnoreCase))
                {
                    return member;
                }
            }
#endif

            foreach (var member in members)
            {
                if (String.Equals(member.Name, columnName, StringComparison.OrdinalIgnoreCase))
                {
                    return member;
                }
            }

            return null;
        }

        private static Expression GetFieldMapperExpression(Context context)
        {
            if (DataTypeMapperStore.TryGetMapper(context.EntityFieldType, out IDataTypeMapper mapper))
            {
                return mapper.GetReaderExpression(
                    context.EntityFieldExpr,
                    context.DataRecordParameterExpr,
                    context.DataRecordIsDbNullMethod,
                    context.FieldIndex);
            }

            return GetValueTypeFieldMapperExpression(context);
        }

        private static Expression GetValueTypeFieldMapperExpression(Context context)
        {
            var readExpr = GetValueTypeReaderExpression(context);

            // entity.<some_field> = <reader_expr>;
            Expression entitySetFieldExpr = Expression.Assign(
                context.EntityFieldExpr,
                readExpr);
#if FEATURE_TYPE_INFO
            if (Nullable.GetUnderlyingType(context.EntityFieldType) != null || !context.EntityFieldType.GetTypeInfo().IsValueType)
#else
            if (Nullable.GetUnderlyingType(context.EntityFieldType) != null || !context.EntityFieldType.IsValueType)
#endif
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

        private static Expression GetValueTypeReaderExpression(Context context)
        {
            if (DataRecordMap.TryGetGetMethod(context.EntityFieldType, out MethodInfo method))
            {
                Expression callReaderExpr = Expression.Call(context.DataRecordParameterExpr, method, Expression.Constant(context.FieldIndex));
#if FEATURE_TYPE_INFO
                if (context.EntityFieldType.GetTypeInfo().IsEnum || Nullable.GetUnderlyingType(context.EntityFieldType) != null)
#else
                if (context.EntityFieldType.IsEnum || Nullable.GetUnderlyingType(context.EntityFieldType) != null)
#endif
                {
                    callReaderExpr = Expression.Convert(callReaderExpr, context.EntityFieldType);
                }

                return callReaderExpr;
            }

            throw new NotSupportedException($"Field of type {context.EntityFieldType} is not supported.");
        }
    }
}
