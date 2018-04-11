using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace Sandwych.Aston.Linq.Expressions
{
    public partial class LinqExpressionFactory
    {
        public Expression CreateMemberAccessNode(Expression objExpr, IEnumerable<string> memberPath)
        {
            var expr = this.MakeMemberAccessExpression(objExpr, memberPath.First());
            foreach (var p in memberPath.Skip(1))
            {
                expr = this.MakeMemberAccessExpression(expr, p);
            }
            return expr;
        }

        private Expression MakeMemberAccessExpression(Expression objExpr, string name)
        {
            if (!this.MemberAccessStrategy.IsAllowed(objExpr, name))
            {
                throw new InvalidOperationException($"Accessing the member '{name}' is not allowed.");
            }

            var propertyExpr = Expression.PropertyOrField(objExpr, name);
            var mi = this.GetPropertyInfo(objExpr.Type, name);
            return Expression.MakeMemberAccess(objExpr, mi);
        }

        private MemberInfo GetPropertyInfo(Type objectType, string propertyName)
        {
            var mi = objectType.GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance);
            if (mi == null)
            {
                throw new InvalidOperationException(
                    $"The type {objectType.FullName} does not have an accessable property that named: {propertyName}");
            }
            return mi;
        }

    }
}
