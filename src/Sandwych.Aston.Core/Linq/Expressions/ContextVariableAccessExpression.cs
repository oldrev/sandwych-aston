using System;
using System.Collections.Generic;
using System.Text;
using System.Linq.Expressions;

namespace Sandwych.Aston.Linq.Expressions;

public class ContextVariableAccessExpression : Expression
{
    public override bool CanReduce => false;

    public Type SymbolType { get; }

    public string VariableName { get; }

    public override ExpressionType NodeType => ExpressionType.Extension;

    public override Type Type { get; }

    public ContextVariableAccessExpression(Type symbolType, string symbol)
    {
        this.Type = symbolType;
        this.VariableName = symbol;
    }

    protected override Expression Accept(ExpressionVisitor visitor)
    {
        return visitor is IBindingEvaluationContextExpressionVisitor ctxVisitor ?
          ctxVisitor.VisitContextVariableAccessExpression(this) : base.Accept(visitor);
    }
}
