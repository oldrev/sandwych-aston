using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Sandwych.Aston.Linq.Expressions
{
    public class LinqExpressionParseContext : AbstractParseContext<Expression>
    {
        public LinqExpressionParseContext(INodeFactory<Expression> nodeFactory, IEnumerable<Symbol> symbols = null)
            : base(nodeFactory, symbols)
        {
        }

        public LinqExpressionParseContext(Type singleParameterType, IEnumerable<Symbol> additionalSymbols = null)
            : base(new LinqExpressionFactory(singleParameterType), singleParameterType, additionalSymbols)
        {
        }
    }
}
