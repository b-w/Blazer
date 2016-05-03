namespace Blazer.Dynamic
{
    using System.Dynamic;
    using System.Linq.Expressions;
    using System.Reflection;

    internal sealed class BlazerDynamicMetaObject : DynamicMetaObject
    {
        static readonly MethodInfo s_getMethod = typeof(BlazerDynamicObject).GetMethod(nameof(BlazerDynamicObject.Get));
        static readonly MethodInfo s_setMethod = typeof(BlazerDynamicObject).GetMethod(nameof(BlazerDynamicObject.Set));

        public BlazerDynamicMetaObject(Expression expression, BlazerDynamicObject value)
            : base(expression, BindingRestrictions.Empty, value)
        {
        }

        public override DynamicMetaObject BindSetMember(SetMemberBinder binder, DynamicMetaObject value)
        {
            return new DynamicMetaObject(
                Expression.Call(
                    Expression.Convert(Expression, LimitType),
                    s_setMethod,
                    Expression.Constant(binder.Name),
                    Expression.Convert(value.Expression, typeof(object))),
                BindingRestrictions.GetTypeRestriction(Expression, LimitType));
        }

        public override DynamicMetaObject BindGetMember(GetMemberBinder binder)
        {
            return new DynamicMetaObject(
                Expression.Call(
                    Expression.Convert(Expression, LimitType),
                    s_getMethod,
                    Expression.Constant(binder.Name)),
                BindingRestrictions.GetTypeRestriction(Expression, LimitType));
        }
    }
}
