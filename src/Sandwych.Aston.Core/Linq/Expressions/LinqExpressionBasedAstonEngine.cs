using System;
using System.Collections.Generic;
using System.Text;
using System.Linq.Expressions;

namespace Sandwych.Aston.Linq.Expressions
{
    public class LinqExpressionBasedAstonEngine : AbstractAstonEngine<Expression>
    {
        protected override INodeFactory<Expression> NodeFactory => throw new NotImplementedException();

        public LinqExpressionBasedAstonEngine(Options options) : base(options)
        {
        }

    }
}
