namespace Blazer
{
#if FEATURE_FORMATTABLE_STRING
    using System;
    using System.Linq;
#endif
    using System.Collections;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq.Expressions;
    using System.Reflection;

    using Blazer.Caching;

    internal static class ParameterFactory
    {
        private const BindingFlags FLAGS_PUBINST = BindingFlags.Instance | BindingFlags.Public;

        internal delegate void ParameterFactoryFunc(IDbCommand command, object parameters);

#if FEATURE_FORMATTABLE_STRING
        internal delegate void ParameterFactoryFuncFormattableString(IDbCommand command, FormattableString commandString);
#endif

        public static void AddParameters(IDbCommand command, object parameters)
        {
            var factory = GetFactory(parameters);
            factory(command, parameters);
        }

#if FEATURE_FORMATTABLE_STRING
        public static void AddParameters(IDbCommand command, FormattableString commandString)
        {
            var factory = GetFactory(commandString);
            factory(command, commandString);
        }

        private static ParameterFactoryFuncFormattableString GetFactory(FormattableString commandString)
        {
            if (ParameterFactoryCache.TryGet(commandString.Format, out var cachedFactory))
            {
                return cachedFactory;
            }

            var context = GetContext();

            var factoryBodyExpressions = new List<Expression>();

            var commandStringParamExpr = Expression.Parameter(typeof(FormattableString), "commandString");

            var names = Enumerable.Range(0, commandString.ArgumentCount)
                            .Select(x => $"@p__blazer__{x}")
                            .ToArray();

            var formattedCommandText = string.Format(commandString.Format, names);
            var commandTextAssignExpr = Expression.Assign(
                Expression.Property(context.CommandExpr, context.CommandCommandTextProperty),
                Expression.Constant(formattedCommandText));
            factoryBodyExpressions.Add(commandTextAssignExpr);

            var namesVarExpr = Expression.Variable(typeof(string[]), "names");
            var namesAssignExpr = Expression.Assign(
                namesVarExpr,
                Expression.Constant(names));
            factoryBodyExpressions.Add(namesAssignExpr);

            var getArgumentMethod = typeof(FormattableString).GetMethod(nameof(FormattableString.GetArgument), FLAGS_PUBINST);

            for (int i = 0; i < commandString.ArgumentCount; i++)
            {
                var paramValueExpr = Expression.Call(
                    commandStringParamExpr,
                    getArgumentMethod,
                    Expression.Constant(i));

                var parameterValue = new ParameterExpressionFactory.ParameterValue
                {
                    Name = names[i],
                    ValueExpr = paramValueExpr,
                    ValueType = commandString.GetArgument(i).GetType()
                };

                factoryBodyExpressions.Add(ParameterExpressionFactory.GetExpression(context, parameterValue));
            }

            var lambdaBlockExpr = Expression.Block(
                new[] { namesVarExpr },
                factoryBodyExpressions);
            var lambdaExpr = Expression.Lambda<ParameterFactoryFuncFormattableString>(lambdaBlockExpr, context.CommandExpr, commandStringParamExpr);
            var lambda = lambdaExpr.Compile();

            ParameterFactoryCache.Add(commandString.Format, lambda);

            return lambda;
        }
#endif

        private static ParameterFactoryFunc GetFactory(object parameters)
        {
            var parametersType = parameters.GetType();

            if (ParameterFactoryCache.TryGet(parametersType, out ParameterFactoryFunc cachedFactory))
            {
                return cachedFactory;
            }

            var context = GetContext();

            var factoryBodyExpressions = new List<Expression>();

            var untypedParamsExpr = Expression.Parameter(typeof(object), "parameters");
            var typedParamsVarExpr = Expression.Variable(parametersType);
            var typedParamsAssignExpr = Expression.Assign(
                typedParamsVarExpr,
                Expression.Convert(untypedParamsExpr, parametersType));
            factoryBodyExpressions.Add(typedParamsAssignExpr);

            context.ParametersExpr = typedParamsVarExpr;

            foreach (var property in parametersType.GetProperties(FLAGS_PUBINST))
            {
                factoryBodyExpressions.Add(ParameterExpressionFactory.GetExpression(context, property));
            }

            var lambdaBlockExpr = Expression.Block(
                new[] { typedParamsVarExpr },
                factoryBodyExpressions);
            var lambdaExpr = Expression.Lambda<ParameterFactoryFunc>(lambdaBlockExpr, context.CommandExpr, untypedParamsExpr);
            var lambda = lambdaExpr.Compile();

            ParameterFactoryCache.Add(parametersType, lambda);

            return lambda;
        }

        private static ParameterExpressionFactory.Context GetContext()
        {
            var commandType = typeof(IDbCommand);
            var commandCreateParamMethod = commandType.GetMethod(nameof(IDbCommand.CreateParameter), FLAGS_PUBINST);
            var commandCommandTextProperty = commandType.GetProperty(nameof(IDbCommand.CommandText), FLAGS_PUBINST);
            var commandParamExpr = Expression.Parameter(commandType, "command");

            var commandParamsType = typeof(IList);
            var commandParamsProperty = commandType.GetProperty(nameof(IDbCommand.Parameters), FLAGS_PUBINST);
            var commandParamsAddMethod = commandParamsType.GetMethod(nameof(IList.Add), FLAGS_PUBINST);

            var paramType = typeof(IDataParameter);
            var paramDbTypeProperty = paramType.GetProperty(nameof(IDataParameter.DbType), FLAGS_PUBINST);
            var paramDirectionProperty = paramType.GetProperty(nameof(IDataParameter.Direction), FLAGS_PUBINST);
            var paramNameProperty = paramType.GetProperty(nameof(IDataParameter.ParameterName), FLAGS_PUBINST);
            var paramValueProperty = paramType.GetProperty(nameof(IDataParameter.Value), FLAGS_PUBINST);

            return new ParameterExpressionFactory.Context()
            {
                CommandExpr = commandParamExpr,
                CommandParametersProperty = commandParamsProperty,
                CommandCommandTextProperty = commandCommandTextProperty,
                CommandParametersAddMethod = commandParamsAddMethod,
                CreateParamMethod = commandCreateParamMethod,
                ParamDbTypeProperty = paramDbTypeProperty,
                ParamDirectionProperty = paramDirectionProperty,
                ParamNameProperty = paramNameProperty,
                ParamValueProperty = paramValueProperty
            };
        }
    }
}
