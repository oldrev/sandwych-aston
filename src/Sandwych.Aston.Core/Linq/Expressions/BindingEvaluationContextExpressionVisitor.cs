using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Sandwych.Aston.Linq.Expressions
{
    public interface IBindingEvaluationContextExpressionVisitor
    {
        IEvaluationContext Context { get; }

        Expression VisitContextVariableAccessExpression(ContextVariableAccessExpression node);
    }

    public class BindingEvaluationContextExpressionVisitor : ExpressionVisitor, IBindingEvaluationContextExpressionVisitor
    {
        public IEvaluationContext Context { get; }

        public BindingEvaluationContextExpressionVisitor(IEvaluationContext context)
        {
            this.Context = context;
        }

        public virtual Expression VisitContextVariableAccessExpression(ContextVariableAccessExpression node)
        {
            var variable = this.Context.Variables[node.VariableName];
            var newNode = Expression.Constant(variable);
            return newNode;
        }
    }
}
