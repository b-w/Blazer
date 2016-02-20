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

            public ParameterExpression ParametersExpr { get; set; }
        }

        public static Expression GetExpression(Context context, PropertyInfo property)
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
                Expression.Constant(DbType.Int32)); // TODO: Type -> DbType mapping

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

            // { ... }
            var blockExpr = Expression.Block(
                new[] { dbParamVarExpr },
                createParamExpr,
                directionExpr,
                dbTypeExpr,
                nameExpr,
                valueExpr
                );

            return blockExpr;
        }
    }
}
