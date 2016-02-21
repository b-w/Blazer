namespace Blazer
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using System.Text;
    using System.Threading.Tasks;

    internal static class ParameterExpressionFactory
    {
        internal class Context
        {
            public MethodInfo CreateParamMethod { get; set; }

            public PropertyInfo ParamDbTypeProperty { get; set; }

            public PropertyInfo ParamDirectionProperty { get; set; }

            public PropertyInfo ParamNameProperty { get; set; }

            public PropertyInfo ParamValueProperty { get; set; }

            public ParameterExpression CommandExpr { get; set; }

            public PropertyInfo CommandParametersProperty { get; set; }

            public MethodInfo CommandParametersAddMethod { get; set; }

            public ParameterExpression ParametersExpr { get; set; }
        }

        public static Expression GetExpression(Context context, PropertyInfo property)
        {
            DbType dbType;
            if (DbTypeMap.TryGetDbType(property.PropertyType, out dbType))
            {
                return GetExpressionForKnownDbType(context, property, dbType);
            }

            if (typeof(IEnumerable<>).IsAssignableFrom(property.PropertyType))
            {
                var innerType = property.PropertyType.GenericTypeArguments[0];
                if (DbTypeMap.TryGetDbType(innerType, out dbType))
                {

                }

                throw new NotSupportedException($"Collection parameter of type {innerType} is not supported. Only collections of known data types are supported.");
            }

            throw new NotSupportedException($"Parameter of type {property.PropertyType} is not supported.");
        }

        static Expression GetExpressionForKnownDbType(Context context, PropertyInfo property, DbType dbType)
        {
            // IDbDataParameter dataParam;
            var dbParamVarExpr = Expression.Variable(typeof(IDbDataParameter));

            // dataParam = command.CreateParameter();
            var createParamExpr = Expression.Assign(
                dbParamVarExpr,
                Expression.Call(context.CommandExpr, context.CreateParamMethod));

            // dataParam.Direction = ParameterDirection.Input;
            var directionExpr = Expression.Assign(
                Expression.Property(dbParamVarExpr, context.ParamDirectionProperty),
                Expression.Constant(ParameterDirection.Input));

            // dataParam.DbType = <some_type>;
            var dbTypeExpr = Expression.Assign(
                Expression.Property(dbParamVarExpr, context.ParamDbTypeProperty),
                Expression.Constant(dbType));

            // dataParam.ParameterName = "@<prop_name>";
            var nameExpr = Expression.Assign(
                Expression.Property(dbParamVarExpr, context.ParamNameProperty),
                Expression.Constant("@" + property.Name));

            // dataParam.Value = <prop_value>;
            var valueExpr = Expression.Assign(
                Expression.Property(dbParamVarExpr, context.ParamValueProperty),
                Expression.Convert(
                    Expression.Property(context.ParametersExpr, property),
                    typeof(object)));

            // command.Parameters.Add(dataParam);
            var addParamExpr = Expression.Call(
                Expression.Property(context.CommandExpr, context.CommandParametersProperty),
                context.CommandParametersAddMethod,
                dbParamVarExpr);

            // { ... }
            var blockExpr = Expression.Block(
                new[] { dbParamVarExpr },
                createParamExpr,
                directionExpr,
                dbTypeExpr,
                nameExpr,
                valueExpr,
                addParamExpr
                );

            return blockExpr;
        }


    }
}
