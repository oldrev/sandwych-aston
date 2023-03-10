using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Sandwych.Aston.Linq.Expressions;

public class LinqExpressionNodeClrTypeEvaluator : INodeClrTypeEvaluator<Expression>
{
    public Type Evaluate(Expression node) => node.Type;
}
