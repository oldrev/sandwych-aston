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
        public IMemberAccessStrategy<Expression> MemberAccessStrategy { get; }

        public LinqExpressionFactory(IMemberAccessStrategy<Expression> memberAccessStrategy)
        {
            this.MemberAccessStrategy = memberAccessStrategy ?? throw new ArgumentNullException(nameof(memberAccessStrategy));
        }

        public LinqExpressionFactory(Type rootObjectType)
        {
            this.MemberAccessStrategy = new SafeMemberAccessStrategy<Expression>(new LinqExpressionNodeClrTypeEvaluator());
            this.MemberAccessStrategy.Register(rootObjectType, "*");
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

        public virtual Expression CreateStringLiteralNode(string value) =>
            Expression.Constant(value);

        public virtual Expression CreateVectorNode(IEnumerable<Expression> items) =>
            Expression.NewArrayInit(typeof(object), items);

        public virtual Expression CreateListEvaluationNode(string funcName, IEnumerable<Expression> args)
        {
            if (funcName == "eval")
            {
                if (args.Count() == 1)
                {
                    return args.First();
                }
                else
                {
                    throw new SyntaxErrorException();
                }
            }
            else if (funcName == "if" || funcName == "?")
            {
                if (args.Count() == 3)
                {
                    return Expression.Condition(args.First(), args.Skip(1).First(), args.Skip(2).First());
                }
                else
                {

                    throw new SyntaxErrorException();
                }
            }
            else if (funcName == "add" || funcName == "+")
            {
                return args.Aggregate((e1, e2) => Expression.Add(e1, e2));
            }
            else if (funcName == "sub" || funcName == "-")
            {
                return args.Aggregate((e1, e2) => Expression.Subtract(e1, e2));
            }
            else if (funcName == "mul" || funcName == "*")
            {
                return args.Aggregate((e1, e2) => Expression.Multiply(e1, e2));
            }
            else if (funcName == "div" || funcName == "/")
            {
                return args.Aggregate((e1, e2) => Expression.Divide(e1, e2));
            }
            else if (funcName == "mod" || funcName == "%")
            {
                return args.Aggregate((e1, e2) => Expression.Modulo(e1, e2));
            }
            else if (funcName == "eq" || funcName == "==")
            {
                return args.Aggregate((e1, e2) => Expression.Equal(e1, e2));
            }
            else if (funcName == "ne" || funcName == "!=")
            {
                return args.Aggregate((e1, e2) => Expression.NotEqual(e1, e2));
            }
            else if (funcName == "lt" || funcName == "<")
            {
                return Expression.LessThan(args.First(), args.Skip(1).First());
            }
            else if (funcName == "le" || funcName == "<=")
            {
                return Expression.LessThanOrEqual(args.First(), args.Skip(1).First());
            }
            else if (funcName == "gt" || funcName == ">")
            {
                return Expression.GreaterThan(args.First(), args.Skip(1).First());
            }
            else if (funcName == "ge" || funcName == ">=")
            {
                return Expression.GreaterThanOrEqual(args.First(), args.Skip(1).First());
            }
            else if (funcName == "not")
            {
                return Expression.Not(args.First());
            }
            else if (funcName == "and")
            {
                return args.Aggregate((e1, e2) => Expression.AndAlso(e1, e2));
            }
            else if (funcName == "or")
            {
                return args.Aggregate((e1, e2) => Expression.OrElse(e1, e2));
            }
            else if (funcName == "concat")
            {
                throw new NotSupportedException();
            }
            else if (funcName == "print")
            {
                var method = typeof(Console).GetMethod(nameof(Console.WriteLine), new Type[] { typeof(object) });
                return Expression.Call(method, Expression.Constant(args.First()));
            }
            else
            {
                throw new NotSupportedException();
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
