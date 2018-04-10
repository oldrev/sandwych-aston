using Sandwych.Aston.Linq.Expressions;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Sandwych.Aston.Tests
{

    public abstract class ExpressionBasedTest
    {
        protected bool TryEvaluate<T>(string expression, out T result)
        {
            var parser = new AstonParser<Expression>(new LinqExpressionParseContext(typeof(T)));
            var parsed = parser.Parse(expression);
            if (parsed.Success)
            {
                var expr = Expression.Lambda(parsed.Value);
                var dg = expr.Compile();
                result = (T)dg.DynamicInvoke();
                return true;
            }
            else
            {
                result = default(T);
                return false;
            }
        }

    }
}
