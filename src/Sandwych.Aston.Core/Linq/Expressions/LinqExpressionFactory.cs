using System;
using System.Collections.Generic;
using System.Text;
using System.Linq.Expressions;
using System.Linq;
using System.Reflection;

namespace Sandwych.Aston.Linq.Expressions
{
    public partial class LinqExpressionFactory : INodeFactory<Expression>
    {
        private static readonly IEnumerable<Expression> EmptyArguments = new Expression[] { };

        public IMemberAccessStrategy<Expression> MemberAccessStrategy { get; }

        public LinqExpressionFactory(IMemberAccessStrategy<Expression> memberAccessStrategy) : this()
        {
            this.MemberAccessStrategy = memberAccessStrategy ?? throw new ArgumentNullException(nameof(memberAccessStrategy));
        }

        public LinqExpressionFactory(Type rootObjectType) : this()
        {
            this.MemberAccessStrategy = new SafeMemberAccessStrategy<Expression>(new LinqExpressionNodeClrTypeEvaluator());
            this.MemberAccessStrategy.Register(rootObjectType, "*");
        }

        private LinqExpressionFactory()
        {
            this.RegisterBuiltinFunctions();
        }

        private void RegisterBuiltinFunctions()
        {

        }

        public virtual Expression CreateNullLiteralNode() =>
            Expression.Constant(null);

        public virtual Expression CreateBooleanLiteralNode(bool value) =>
            Expression.Constant(value);

        public virtual Expression CreateIntegerLiteralNode(int value) =>
            Expression.Constant(value);

        public virtual Expression CreateLongIntegerLiteralNode(long value) =>
            Expression.Constant(value);

        public virtual Expression CreateDoubleLiteralNode(double value) =>
            Expression.Constant(value);

        public virtual Expression CreateFloatLiteralNode(float value) =>
            Expression.Constant(value);

        public Expression CreateDecimalLiteralNode(decimal value) =>
            Expression.Constant(value);

        public Expression CreateGuidLiteralNode(Guid value) =>
            Expression.Constant(value);

        public Expression CreateDateTimeLiteralNode(DateTime value) =>
            Expression.Constant(value);

        public Expression CreateDateTimeOffsetLiteralNode(DateTimeOffset value) =>
            Expression.Constant(value);

        public virtual Expression CreateStringLiteralNode(string value) =>
            Expression.Constant(value);

        public virtual Expression CreateVectorNode(IEnumerable<Expression> items) =>
            Expression.NewArrayInit(typeof(object), items);

        public virtual Expression CreateListEvaluationNode(IParseContext<Expression> context, string funcName, IEnumerable<Expression> args)
        {
            if (context.Functions.TryGetValue(funcName, out var func))
            {
                args = args ?? EmptyArguments;
                var nargs = args.Count();

                if (func.IsParameterVariadic)
                {
                    if (nargs < func.ParametersCount)
                    {
                        throw new SyntaxErrorException();
                    }
                }
                else
                {
                    if (nargs != func.ParametersCount)
                    {
                        throw new SyntaxErrorException();
                    }

                }

                return func.InvocationFactory.Invoke(args);
            }
            else
            {
                throw new SyntaxErrorException();
            }
        }

        public virtual Expression CreateCustomLiteralNode(object value)
        {
            throw new NotImplementedException();
        }

        public virtual Expression CreateParameterNode(Type type, string name) =>
            Expression.Parameter(type, name);


        public virtual Expression CreateSymbolAccessNode(IParseContext<Expression> context, string symbolName)
        {
            if (string.IsNullOrEmpty(symbolName))
            {
                throw new ArgumentNullException(nameof(symbolName));
            }

            if (context.Symbols.TryGetValue(symbolName, out var symbol))
            {
                if (symbol.Type == SymbolType.ContextVariable)
                {
                    return new ContextVariableAccessExpression(symbol.ClrType, symbol.Name);
                }
                else if (symbol.Type == SymbolType.Parameter)
                {
                    return context.Parameters[symbolName];
                }
                else
                {
                    throw new NotSupportedException();
                }
            }
            else
            {
                throw new ArgumentOutOfRangeException(symbolName);
            }
        }
    }
}
