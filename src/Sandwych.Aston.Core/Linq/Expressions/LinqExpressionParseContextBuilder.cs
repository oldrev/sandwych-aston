using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Sandwych.Aston.Linq.Expressions
{
    public class LinqExpressionParseContextBuilder
    {
        public LinqExpressionParseContextBuilder()
        {
            this.BuiltinFunctionStrategy = DefaultBuiltinFunctionsStrategy.Instance.Value;
        }

        public IBuiltinFunctionStrategy<Expression> BuiltinFunctionStrategy { get; private set; }

        public ParseContext<Expression> Build()
        {
            var ctx = new ParseContext<Expression>();
            this.BuiltinFunctionStrategy.RegisterBuiltinFunctions(ctx);

            return ctx;
        }

        public LinqExpressionParseContextBuilder ReplaceBuiltinFunctionStrategy(IBuiltinFunctionStrategy<Expression> strategy)
        {
            this.BuiltinFunctionStrategy = strategy ?? throw new ArgumentNullException(nameof(strategy));
            return this;
        }
    }
}
