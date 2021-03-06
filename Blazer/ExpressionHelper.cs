﻿namespace Blazer
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq.Expressions;
#if NETSTANDARD
    using System.Reflection;
#endif

    internal static class ExpressionHelper
    {
        public static Expression For(
            ParameterExpression loopVarExpr,
            Expression loopVarValueExpr,
            Expression loopConditionExpr,
            Expression loopIncrementExpr,
            Expression loopBodyExpr)
        {
            var loopVarAssignExpr = Expression.Assign(loopVarExpr, loopVarValueExpr);
            var loopBreakLabelExpr = Expression.Label("break");

            return Expression.Block(
                new[] { loopVarExpr },
                loopVarAssignExpr,
                Expression.Loop(
                    Expression.IfThenElse(
                        loopConditionExpr,
                        Expression.Block(
                            loopBodyExpr,
                            loopIncrementExpr
                        ),
                        Expression.Break(loopBreakLabelExpr)
                    ),
                loopBreakLabelExpr)
            );
        }

        public static Expression ForEach(
            ParameterExpression loopVarExpr,
            Expression collectionExpr,
            Expression loopBodyExpr)
        {
            var collectionType = loopVarExpr.Type;
            var enumerableType = typeof(IEnumerable<>).MakeGenericType(collectionType);
            var enumeratorType = typeof(IEnumerator<>).MakeGenericType(collectionType);
            var enumeratorVarExpr = Expression.Variable(enumeratorType, "enumerator");
            var enumeratorAssignExpr = Expression.Assign(
                enumeratorVarExpr,
                Expression.Call(
                    collectionExpr,
                    enumerableType.GetMethod(nameof(IEnumerable.GetEnumerator))));
            var moveNextCallExpr = Expression.Call(enumeratorVarExpr, typeof(IEnumerator).GetMethod(nameof(IEnumerator.MoveNext)));
            var disposeCallExpr = Expression.Call(enumeratorVarExpr, typeof(IDisposable).GetMethod(nameof(IDisposable.Dispose)));
            var loopBreakLabelExpr = Expression.Label("break");

            return Expression.Block(
                new[] { enumeratorVarExpr },
                enumeratorAssignExpr,
                Expression.Loop(
                    Expression.IfThenElse(
                        Expression.Equal(moveNextCallExpr, Expression.Constant(true)),
                        Expression.Block(
                            new[] { loopVarExpr },
                            Expression.Assign(loopVarExpr, Expression.Property(enumeratorVarExpr, nameof(IEnumerator.Current))),
                            loopBodyExpr
                        ),
                        Expression.Break(loopBreakLabelExpr)
                    ),
                loopBreakLabelExpr),
                disposeCallExpr
            );
        }
    }
}
