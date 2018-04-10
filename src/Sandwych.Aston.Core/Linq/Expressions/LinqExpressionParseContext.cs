using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
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

        protected override void OnRegisterBuiltinFunctions()
        {
            this.RegisterFunction("eval", false, 1, args => args.Single());
            this.RegisterFunction("if", false, 3, args => Expression.Condition(args.First(), args.Second(), args.Third()));

            // arithmetic operation related functions:
            this.RegisterFunction("add", true, 2, args => args.Aggregate((e1, e2) => Expression.Add(e1, e2)));
            this.RegisterFunction("sub", true, 2, args => args.Aggregate((e1, e2) => Expression.Subtract(e1, e2)));
            this.RegisterFunction("mul", true, 2, args => args.Aggregate((e1, e2) => Expression.Multiply(e1, e2)));
            this.RegisterFunction("div", true, 2, args => args.Aggregate((e1, e2) => Expression.Divide(e1, e2)));
            this.RegisterFunction("pow", true, 2, args => args.Aggregate((e1, e2) => Expression.Power(e1, e2)));
            this.RegisterFunction("mod", true, 2, args => args.Aggregate((e1, e2) => Expression.Modulo(e1, e2)));

            //comparsion functions:
            this.RegisterFunction("eq", true, 2, args => args.Aggregate((e1, e2) => Expression.Equal(e1, e2)));
            this.RegisterFunction("ne", true, 2, args => args.Aggregate((e1, e2) => Expression.NotEqual(e1, e2)));
            this.RegisterFunction("lt", true, 2, args => args.Aggregate((e1, e2) => Expression.LessThan(e1, e2)));
            this.RegisterFunction("le", true, 2, args => args.Aggregate((e1, e2) => Expression.LessThanOrEqual(e1, e2)));
            this.RegisterFunction("gt", true, 2, args => args.Aggregate((e1, e2) => Expression.GreaterThan(e1, e2)));
            this.RegisterFunction("ge", true, 2, args => args.Aggregate((e1, e2) => Expression.GreaterThanOrEqual(e1, e2)));

            //unary functions:
            this.RegisterFunction("neg", false, 1, args => Expression.Negate(args.Single()));

            //boolean functions:
            this.RegisterFunction("and", true, 2, args => args.Aggregate((e1, e2) => Expression.AndAlso(e1, e2)));
            this.RegisterFunction("or", true, 2, args => args.Aggregate((e1, e2) => Expression.OrElse(e1, e2)));
            this.RegisterFunction("not", false, 1, args => args.Aggregate((e1, e2) => Expression.Not(e1)));
            this.RegisterFunction("between", false, 3,
                args => Expression.AndAlso(
                            Expression.LessThanOrEqual(args.First(), args.Second()),
                            Expression.GreaterThanOrEqual(args.First(), args.Third())));

            //collection related functions:
            this.RegisterFunction("contains", false, 2, args =>
            {
                var enumerableExpression = args.First();
                var elementExpression = args.Second();
                var containsMethod = new Func<IEnumerable<object>, object, bool>(Enumerable.Contains)
                    .GetMethodInfo()
                    .GetGenericMethodDefinition();
                var method = containsMethod.MakeGenericMethod(elementExpression.Type);
                return Expression.Call(null, method, enumerableExpression, elementExpression);
            });

            this.RegisterFunction("in", false, 2, args =>
            {
                var enumerableExpression = args.Second();
                var elementExpression = args.First();
                var containsMethod = new Func<IEnumerable<object>, object, bool>(Enumerable.Contains)
                    .GetMethodInfo()
                    .GetGenericMethodDefinition();
                var method = containsMethod.MakeGenericMethod(elementExpression.Type);
                return Expression.Call(null, method, enumerableExpression, elementExpression);
            });

            //string related functions
            this.RegisterFunction("starts-with", false, 2, args =>
            {
                var method = typeof(string).GetMethod(nameof(string.StartsWith), new Type[] { typeof(string) });
                return Expression.Call(args.First(), method, args.Second());
            });

            this.RegisterFunction("ends-with", false, 2, args =>
            {
                var method = typeof(string).GetMethod(nameof(string.EndsWith), new Type[] { typeof(string) });
                return Expression.Call(args.First(), method, args.Second());
            });

            this.RegisterFunction("to-lower", false, 1, args =>
            {
                var method = typeof(string).GetMethod(nameof(string.ToLowerInvariant));
                return Expression.Call(args.First(), method);
            });

            this.RegisterFunction("to-upper", false, 1, args =>
            {
                var method = typeof(string).GetMethod(nameof(string.ToUpperInvariant));
                return Expression.Call(args.First(), method);
            });

        }
    }
}
