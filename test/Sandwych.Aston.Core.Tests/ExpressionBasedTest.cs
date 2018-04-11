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
            var parseContextBuilder = new LinqExpressionParseContextBuilder();
            var parseContext = parseContextBuilder.Build();
            var nodeFactory = new LinqExpressionFactory(typeof(T));
            var parser = new AstonParser<Expression>(nodeFactory);
            if (parser.TryParse(expression, out var parsed))
            {
                var expr = Expression.Lambda(parsed);
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
