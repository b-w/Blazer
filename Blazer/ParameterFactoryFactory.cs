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

    public static class ParameterFactoryFactory
    {
        private const BindingFlags BindingFlags_InstanceProp = BindingFlags.Instance | BindingFlags.Public;

        public delegate void ParameterFactory(IDbCommand command, object parameters);

        public static ParameterFactory GetFactory(object parameters)
        {
            //IDbCommand command = null;
            //var par = command.CreateParameter();
            //par.DbType = DbType.Int32;
            //par.Direction = ParameterDirection.Input;
            //par.ParameterName = "@Id";
            //par.Value = 42;

            var commandType = typeof(IDbCommand);
            var dataParameterType = typeof(IDataParameter);
            var parametersType = parameters.GetType();

            var createParamMethod = commandType.GetMethod("CreateParameter", BindingFlags_InstanceProp);
            var paramDbTypeProperty = dataParameterType.GetProperty("DbType", BindingFlags_InstanceProp);
            var paramDirectionProperty = dataParameterType.GetProperty("Direction", BindingFlags_InstanceProp);
            var paramNameProperty = dataParameterType.GetProperty("ParameterName", BindingFlags_InstanceProp);
            var paramValueProperty = dataParameterType.GetProperty("Value", BindingFlags_InstanceProp);

            // (command, parameters) =>
            var commandParamExpr = Expression.Parameter(commandType, "command");
            var parametersParamExpr = Expression.Parameter(typeof(object), "parameters");

            var factoryBodyExpressions = new List<Expression>();

            // var parametersTyped = (<params_type>)parameters;
            var typedParamsVarExpr = Expression.Variable(parametersType);
            var typedParamsAssignExpr = Expression.Assign(
                typedParamsVarExpr,
                Expression.Convert(parametersParamExpr, parametersType));
            factoryBodyExpressions.Add(typedParamsAssignExpr);

            foreach (var prop in parametersType.GetProperties(BindingFlags_InstanceProp))
            {
                // IDbDataParameter dataParam;
                var dbParamVarExpr = Expression.Variable(typeof(IDbDataParameter));

                // dataParam = command.CreateParameter();
                var createParamExpr = Expression.Assign(
                    dbParamVarExpr,
                    Expression.Call(commandParamExpr, createParamMethod));

                // dataParam.Direction = ParameterDirection.Input;
                var directionExpr = Expression.Assign(
                    Expression.Property(dbParamVarExpr, paramDirectionProperty),
                    Expression.Constant(ParameterDirection.Input));

                // dataParam.DbType = <some_type>;
                var dbTypeExpr = Expression.Assign(
                    Expression.Property(dbParamVarExpr, paramDbTypeProperty),
                    Expression.Constant(DbType.Int32)); // TODO: Type -> DbType mapping

                // dataParam.ParameterName = "@<prop_name>";
                var nameExpr = Expression.Assign(
                    Expression.Property(dbParamVarExpr, paramNameProperty),
                    Expression.Constant("@" + prop.Name));

                // dataParam.Value = <prop_value>;
                var valueExpr = Expression.Assign(
                    Expression.Property(dbParamVarExpr, paramValueProperty),
                    Expression.Convert(
                        Expression.Property(typedParamsVarExpr, prop),
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

                factoryBodyExpressions.Add(blockExpr);
            }
            var lambdaBlockExpr = Expression.Block(
                new[] { typedParamsVarExpr },
                factoryBodyExpressions);
            var lambdaExpr = Expression.Lambda<ParameterFactory>(lambdaBlockExpr, commandParamExpr, parametersParamExpr);
            var lambda = lambdaExpr.Compile();

            return lambda;
        }
    }
}
