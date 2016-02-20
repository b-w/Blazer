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
            var commandCreateParamMethod = commandType.GetMethod("CreateParameter", BindingFlags_InstanceProp);
            var commandParamExpr = Expression.Parameter(commandType, "command");

            var paramType = typeof(IDataParameter);
            var paramDbTypeProperty = paramType.GetProperty("DbType", BindingFlags_InstanceProp);
            var paramDirectionProperty = paramType.GetProperty("Direction", BindingFlags_InstanceProp);
            var paramNameProperty = paramType.GetProperty("ParameterName", BindingFlags_InstanceProp);
            var paramValueProperty = paramType.GetProperty("Value", BindingFlags_InstanceProp);

            var factoryBodyExpressions = new List<Expression>();

            var parametersType = parameters.GetType();
            var untypedParamsExpr = Expression.Parameter(typeof(object), "parameters");
            var typedParamsVarExpr = Expression.Variable(parametersType);
            var typedParamsAssignExpr = Expression.Assign(
                typedParamsVarExpr,
                Expression.Convert(untypedParamsExpr, parametersType));
            factoryBodyExpressions.Add(typedParamsAssignExpr);

            var context = new ParameterExpressionFactory.Context()
            {
                CommandExpr = commandParamExpr,
                ParametersExpr = typedParamsVarExpr,
                CreateParamMethod = commandCreateParamMethod,
                ParamDbTypeProperty = paramDbTypeProperty,
                ParamDirectionProperty = paramDirectionProperty,
                ParamNameProperty = paramNameProperty,
                ParamValueProperty = paramValueProperty
            };

            foreach (var property in parametersType.GetProperties(BindingFlags_InstanceProp))
            {
                factoryBodyExpressions.Add(ParameterExpressionFactory.GetExpression(context, property));
            }
            var lambdaBlockExpr = Expression.Block(
                new[] { typedParamsVarExpr },
                factoryBodyExpressions);
            var lambdaExpr = Expression.Lambda<ParameterFactory>(lambdaBlockExpr, commandParamExpr, untypedParamsExpr);
            var lambda = lambdaExpr.Compile();

            return lambda;
        }
    }
}
