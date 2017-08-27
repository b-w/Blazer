namespace Blazer
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using System.Text;
    using System.Text.RegularExpressions;

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

            public PropertyInfo CommandCommandTextProperty { get; set; }

            public ParameterExpression ParametersExpr { get; set; }
        }

#if FEATURE_FORMATTABLE_STRING
        internal class ParameterValue
        {
            public string Name { get; set; }

            public Type ValueType { get; set; }

            public Expression ValueExpr { get; set; }
        }

        public static Expression GetExpression(Context context, ParameterValue value)
        {
            DbType dbType;
            if (DbTypeStore.TryGetDbType(value.ValueType, out dbType))
            {
                return GetExpressionForKnownDbType(context, value, dbType);
            }

#if FEATURE_TYPE_INFO
            var collectionInterfaceType = value.ValueType.GetInterfaces().FirstOrDefault(x => x.GetTypeInfo().IsGenericType && x.GetGenericTypeDefinition() == typeof(IEnumerable<>));
#else
            var collectionInterfaceType = value.ValueType.GetInterfaces().FirstOrDefault(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(IEnumerable<>));
#endif
            if (collectionInterfaceType != null)
            {
#if FEATURE_GENERIC_TYPE_ARGS
                var innerType = collectionInterfaceType.GenericTypeArguments[0];
#else
                var innerType = collectionInterfaceType.GetGenericArguments()[0];
#endif
                if (DbTypeStore.TryGetDbType(innerType, out dbType))
                {
                    return GetExpressionForCollectionType(context, value, innerType, dbType);
                }

                throw new NotSupportedException($"Collection parameter of type {innerType} is not supported. Only collections of known data types are supported.");
            }

            throw new NotSupportedException($"Parameter of type {value.ValueType} is not supported.");
        }

        private static Expression GetExpressionForKnownDbType(Context context, ParameterValue value, DbType dbType)
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

            // dataParam.ParameterName = "<param_name>";
            var nameExpr = Expression.Assign(
                Expression.Property(dbParamVarExpr, context.ParamNameProperty),
                Expression.Constant(value.Name));

            // dataParam.Value = <prop_value>;
            var valueExpr = Expression.Assign(
                Expression.Property(dbParamVarExpr, context.ParamValueProperty),
                Expression.Convert(
                    value.ValueExpr,
                    typeof(object)));

            // command.Parameters.Add(dataParam);
            var addParamExpr = Expression.Call(
                Expression.Property(context.CommandExpr, context.CommandParametersProperty),
                context.CommandParametersAddMethod,
                dbParamVarExpr);

            return Expression.Block(
                new[] { dbParamVarExpr },
                createParamExpr,
                directionExpr,
                dbTypeExpr,
                nameExpr,
                valueExpr,
                addParamExpr
                );
        }

        private static Expression GetExpressionForCollectionType(Context context, ParameterValue value, Type collectionType, DbType dbType)
        {
            var propertyVariableName = value.Name;

            var sbType = typeof(StringBuilder);
            var sbAppendStringMethod = sbType.GetMethod("Append", new[] { typeof(string) });
            var sbAppendIntMethod = sbType.GetMethod("Append", new[] { typeof(int) });
            var sbToStringMethod = sbType.GetMethod("ToString", new Type[] { });
            var stringConcatMethod = typeof(string).GetMethod("Concat", new[] { typeof(object), typeof(object) });
            var rxEscapeMethod = typeof(Regex).GetMethod("Escape", new[] { typeof(string) });
            var rxReplaceMethod = typeof(Regex).GetMethod("Replace", new[] { typeof(string), typeof(string), typeof(string) });

            // StringBuilder sb;
            var sbVarExpr = Expression.Variable(sbType);

            // sb = new StringBuilder();
            var sbNewExpr = Expression.Assign(
                sbVarExpr,
                Expression.New(sbType));

            // sb.Append("(");
            var sbOpenBracketExpr = Expression.Call(
                sbVarExpr,
                sbAppendStringMethod,
                Expression.Constant("("));

            // int i;
            var iVarExpr = Expression.Variable(typeof(int));

            // i = 0;
            var iInitExpr = Expression.Assign(iVarExpr, Expression.Constant(0));

            // foreach (var <item> in <collection>)
            // {
            var loopVarExpr = Expression.Variable(collectionType);

            //      if (i > 0)
            //      {
            //          sb.Append(",");
            //      }
            var sbAppendCommaExpr = Expression.IfThen(
                Expression.GreaterThan(iVarExpr, Expression.Constant(0)),
                Expression.Call(
                    sbVarExpr,
                    sbAppendStringMethod,
                    Expression.Constant(",")));

            //      sb.Append("<prop_name>");
            var sbAppendPropNameExpr = Expression.Call(
                sbVarExpr,
                sbAppendStringMethod,
                Expression.Constant(propertyVariableName));

            //      sb.Append(i);
            var sbAppendIExpr = Expression.Call(
                sbVarExpr,
                sbAppendIntMethod,
                iVarExpr);

            //      IDbDataParameter dataParam;
            var dbParamVarExpr = Expression.Variable(typeof(IDbDataParameter));

            //      dataParam = command.CreateParameter();
            var createParamExpr = Expression.Assign(
                dbParamVarExpr,
                Expression.Call(context.CommandExpr, context.CreateParamMethod));

            //      dataParam.Direction = ParameterDirection.Input;
            var directionExpr = Expression.Assign(
                Expression.Property(dbParamVarExpr, context.ParamDirectionProperty),
                Expression.Constant(ParameterDirection.Input));

            //      dataParam.DbType = <some_type>;
            var dbTypeExpr = Expression.Assign(
                Expression.Property(dbParamVarExpr, context.ParamDbTypeProperty),
                Expression.Constant(dbType));

            //      dataParam.ParameterName = "@<prop_name>" + i;
            var nameExpr = Expression.Assign(
                Expression.Property(dbParamVarExpr, context.ParamNameProperty),
                Expression.Call(
                    stringConcatMethod,
                    Expression.Constant(propertyVariableName),
                    Expression.Convert(iVarExpr, typeof(object))));

            //      dataParam.Value = <item>;
            var valueExpr = Expression.Assign(
                Expression.Property(dbParamVarExpr, context.ParamValueProperty),
                Expression.Convert(
                    loopVarExpr,
                    typeof(object)));

            //      command.Parameters.Add(dataParam);
            var addParamExpr = Expression.Call(
                Expression.Property(context.CommandExpr, context.CommandParametersProperty),
                context.CommandParametersAddMethod,
                dbParamVarExpr);

            //      i++;
            var iIncrementExpr = Expression.Increment(iVarExpr);

            // }
            var loopExpr = ExpressionHelper.ForEach(
                loopVarExpr,
                value.ValueExpr,
                Expression.Block(
                    new[] { dbParamVarExpr },
                    sbAppendCommaExpr,
                    sbAppendPropNameExpr,
                    sbAppendIExpr,
                    createParamExpr,
                    directionExpr,
                    dbTypeExpr,
                    nameExpr,
                    valueExpr,
                    addParamExpr,
                    iIncrementExpr
                    ));

            // sb.Append(")");
            var sbCloseBracketExpr = Expression.Call(
                sbVarExpr,
                sbAppendStringMethod,
                Expression.Constant(")"));

            // string regexPattern;
            var rxVarExpr = Expression.Variable(typeof(string));

            // regexPattern = @"\(\s*(" + Regex.Escape("@<prop_name>") + @")\s*\)";
            var rxAssignExpr = Expression.Assign(
                rxVarExpr,
                Expression.Call(
                    stringConcatMethod,
                    Expression.Constant(@"\(\s*("),
                    Expression.Call(
                        stringConcatMethod,
                        Expression.Call(
                            rxEscapeMethod,
                            Expression.Constant(propertyVariableName)),
                        Expression.Constant(@")\s*\)"))));

            // command.CommandText = Regex.Replace(command.CommandText, regexPattern, sb.ToString());
            var cmdTextAssignExpr = Expression.Assign(
                Expression.Property(context.CommandExpr, context.CommandCommandTextProperty),
                Expression.Call(
                    rxReplaceMethod,
                    Expression.Property(context.CommandExpr, context.CommandCommandTextProperty),
                    rxVarExpr,
                    Expression.Call(
                        sbVarExpr,
                        sbToStringMethod)));

            return Expression.Block(
                new[] { sbVarExpr, iVarExpr, loopVarExpr, rxVarExpr },
                sbNewExpr,
                sbOpenBracketExpr,
                iInitExpr,
                loopExpr,
                sbCloseBracketExpr,
                rxAssignExpr,
                cmdTextAssignExpr);
        }
#endif

        public static Expression GetExpression(Context context, PropertyInfo property)
        {
            DbType dbType;
            if (DbTypeStore.TryGetDbType(property.PropertyType, out dbType))
            {
                return GetExpressionForKnownDbType(context, property, dbType);
            }

#if FEATURE_TYPE_INFO
            var collectionInterfaceType = property.PropertyType.GetInterfaces().FirstOrDefault(x => x.GetTypeInfo().IsGenericType && x.GetGenericTypeDefinition() == typeof(IEnumerable<>));
#else
            var collectionInterfaceType = property.PropertyType.GetInterfaces().FirstOrDefault(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(IEnumerable<>));
#endif
            if (collectionInterfaceType != null)
            {
#if FEATURE_GENERIC_TYPE_ARGS
                var innerType = collectionInterfaceType.GenericTypeArguments[0];
#else
                var innerType = collectionInterfaceType.GetGenericArguments()[0];
#endif
                if (DbTypeStore.TryGetDbType(innerType, out dbType))
                {
                    return GetExpressionForCollectionType(context, property, innerType, dbType);
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

        static Expression GetExpressionForCollectionType(Context context, PropertyInfo property, Type collectionType, DbType dbType)
        {
            var propertyVariableName = "@" + property.Name;

            var sbType = typeof(StringBuilder);
            var sbAppendStringMethod = sbType.GetMethod("Append", new[] { typeof(string) });
            var sbAppendIntMethod = sbType.GetMethod("Append", new[] { typeof(int) });
            var sbToStringMethod = sbType.GetMethod("ToString", new Type[] { });
            var stringConcatMethod = typeof(string).GetMethod("Concat", new[] { typeof(object), typeof(object) });
            var rxEscapeMethod = typeof(Regex).GetMethod("Escape", new[] { typeof(string) });
            var rxReplaceMethod = typeof(Regex).GetMethod("Replace", new[] { typeof(string), typeof(string), typeof(string) });

            // StringBuilder sb;
            var sbVarExpr = Expression.Variable(sbType);

            // sb = new StringBuilder();
            var sbNewExpr = Expression.Assign(
                sbVarExpr,
                Expression.New(sbType));

            // sb.Append("(");
            var sbOpenBracketExpr = Expression.Call(
                sbVarExpr,
                sbAppendStringMethod,
                Expression.Constant("("));

            // int i;
            var iVarExpr = Expression.Variable(typeof(int));

            // i = 0;
            var iInitExpr = Expression.Assign(iVarExpr, Expression.Constant(0));

            // foreach (var <item> in <collection>)
            // {
            var loopVarExpr = Expression.Variable(collectionType);

            //      if (i > 0)
            //      {
            //          sb.Append(",");
            //      }
            var sbAppendCommaExpr = Expression.IfThen(
                Expression.GreaterThan(iVarExpr, Expression.Constant(0)),
                Expression.Call(
                    sbVarExpr,
                    sbAppendStringMethod,
                    Expression.Constant(",")));

            //      sb.Append("<prop_name>");
            var sbAppendPropNameExpr = Expression.Call(
                sbVarExpr,
                sbAppendStringMethod,
                Expression.Constant(propertyVariableName));

            //      sb.Append(i);
            var sbAppendIExpr = Expression.Call(
                sbVarExpr,
                sbAppendIntMethod,
                iVarExpr);

            //      IDbDataParameter dataParam;
            var dbParamVarExpr = Expression.Variable(typeof(IDbDataParameter));

            //      dataParam = command.CreateParameter();
            var createParamExpr = Expression.Assign(
                dbParamVarExpr,
                Expression.Call(context.CommandExpr, context.CreateParamMethod));

            //      dataParam.Direction = ParameterDirection.Input;
            var directionExpr = Expression.Assign(
                Expression.Property(dbParamVarExpr, context.ParamDirectionProperty),
                Expression.Constant(ParameterDirection.Input));

            //      dataParam.DbType = <some_type>;
            var dbTypeExpr = Expression.Assign(
                Expression.Property(dbParamVarExpr, context.ParamDbTypeProperty),
                Expression.Constant(dbType));

            //      dataParam.ParameterName = "@<prop_name>" + i;
            var nameExpr = Expression.Assign(
                Expression.Property(dbParamVarExpr, context.ParamNameProperty),
                Expression.Call(
                    stringConcatMethod,
                    Expression.Constant(propertyVariableName),
                    Expression.Convert(iVarExpr, typeof(object))));

            //      dataParam.Value = <item>;
            var valueExpr = Expression.Assign(
                Expression.Property(dbParamVarExpr, context.ParamValueProperty),
                Expression.Convert(
                    loopVarExpr,
                    typeof(object)));

            //      command.Parameters.Add(dataParam);
            var addParamExpr = Expression.Call(
                Expression.Property(context.CommandExpr, context.CommandParametersProperty),
                context.CommandParametersAddMethod,
                dbParamVarExpr);

            //      i++;
            var iIncrementExpr = Expression.Increment(iVarExpr);

            // }
            var loopExpr = ExpressionHelper.ForEach(
                loopVarExpr,
                Expression.Property(context.ParametersExpr, property),
                Expression.Block(
                    new[] { dbParamVarExpr },
                    sbAppendCommaExpr,
                    sbAppendPropNameExpr,
                    sbAppendIExpr,
                    createParamExpr,
                    directionExpr,
                    dbTypeExpr,
                    nameExpr,
                    valueExpr,
                    addParamExpr,
                    iIncrementExpr
                    ));

            // sb.Append(")");
            var sbCloseBracketExpr = Expression.Call(
                sbVarExpr,
                sbAppendStringMethod,
                Expression.Constant(")"));

            // string regexPattern;
            var rxVarExpr = Expression.Variable(typeof(string));

            // regexPattern = @"\(\s*(" + Regex.Escape("@<prop_name>") + @")\s*\)";
            var rxAssignExpr = Expression.Assign(
                rxVarExpr,
                Expression.Call(
                    stringConcatMethod,
                    Expression.Constant(@"\(\s*("),
                    Expression.Call(
                        stringConcatMethod,
                        Expression.Call(
                            rxEscapeMethod,
                            Expression.Constant(propertyVariableName)),
                        Expression.Constant(@")\s*\)"))));

            // command.CommandText = Regex.Replace(command.CommandText, regexPattern, sb.ToString());
            var cmdTextAssignExpr = Expression.Assign(
                Expression.Property(context.CommandExpr, context.CommandCommandTextProperty),
                Expression.Call(
                    rxReplaceMethod,
                    Expression.Property(context.CommandExpr, context.CommandCommandTextProperty),
                    rxVarExpr,
                    Expression.Call(
                        sbVarExpr,
                        sbToStringMethod)));

            var blockExpr = Expression.Block(
                new[] { sbVarExpr, iVarExpr, loopVarExpr, rxVarExpr },
                sbNewExpr,
                sbOpenBracketExpr,
                iInitExpr,
                loopExpr,
                sbCloseBracketExpr,
                rxAssignExpr,
                cmdTextAssignExpr);

            return blockExpr;
        }
    }
}
