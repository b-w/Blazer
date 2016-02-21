namespace Blazer
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using System.Text;
    using System.Threading.Tasks;

    public static class ParameterFactoryFactory
    {
        private const BindingFlags FLAGS_PUBINST = BindingFlags.Instance | BindingFlags.Public;

        public delegate void ParameterFactory(IDbCommand command, object parameters);

        public static ParameterFactory GetFactory(object parameters)
        {
            var commandType = typeof(IDbCommand);
            var commandCreateParamMethod = commandType.GetMethod("CreateParameter", FLAGS_PUBINST);
            var commandParamExpr = Expression.Parameter(commandType, "command");

            var commandParamsType = typeof(IList);
            var commandParamsProperty = commandType.GetProperty("Parameters", FLAGS_PUBINST);
            var commandParamsAddMethod = commandParamsType.GetMethod("Add", FLAGS_PUBINST | BindingFlags.FlattenHierarchy);

            var paramType = typeof(IDataParameter);
            var paramDbTypeProperty = paramType.GetProperty("DbType", FLAGS_PUBINST);
            var paramDirectionProperty = paramType.GetProperty("Direction", FLAGS_PUBINST);
            var paramNameProperty = paramType.GetProperty("ParameterName", FLAGS_PUBINST);
            var paramValueProperty = paramType.GetProperty("Value", FLAGS_PUBINST);

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
                CommandParametersProperty = commandParamsProperty,
                CommandParametersAddMethod = commandParamsAddMethod,
                ParametersExpr = typedParamsVarExpr,
                CreateParamMethod = commandCreateParamMethod,
                ParamDbTypeProperty = paramDbTypeProperty,
                ParamDirectionProperty = paramDirectionProperty,
                ParamNameProperty = paramNameProperty,
                ParamValueProperty = paramValueProperty
            };

            foreach (var property in parametersType.GetProperties(FLAGS_PUBINST))
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
