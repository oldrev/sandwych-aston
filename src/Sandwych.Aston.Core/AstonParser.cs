using System;
using System.Collections.Immutable;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Pidgin;
using static Pidgin.Parser;
using static Pidgin.Parser<char>;
using System.Linq.Expressions;
using System.Reflection;

//syntax:
/* query(where(or(like("Name", "%John%"), gt("Age", "Awesome"))), select("Name", "Id", "Age"), offset(100), limit(100))
 * 
 * 
 * */

namespace Sandwych.Aston
{
    public class AstonParser<TNode>
    {
        public delegate TNode BoundNode(ParseContext<TNode> context);

        private readonly ICustomLiteralsProvider _customLiteralProvider;

        public Parser<char, BoundNode> UnboundRootParser { get; private set; }

        public AstonParser(INodeFactory<TNode> nodeFactory, ICustomLiteralsProvider customLiteralProvider = null)
        {
            _customLiteralProvider = customLiteralProvider;

            // Literals:
            var nullLiteral = String("null")
                .Select(x => new BoundNode(ctx => nodeFactory.CreateNullLiteralValueNode()));

            var trueLiteralParser = String("true").Select(x => true);
            var falseLiteralParser = String("false").Select(x => false);
            var booleanLiteralParser = trueLiteralParser.Or(falseLiteralParser)
                .Select(x => new BoundNode(ctx => nodeFactory.CreateLiteralValueNode(x)));

            var integerLiteral =
                Tokens.IntegerToken
                .Between(SkipWhitespaces)
                .Select(x => new BoundNode(ctx => nodeFactory.CreateLiteralValueNode(x)))
                .Labelled("integer-literal");

            var longIntegerLiteral = Tokens.LongIntegerToken
                .Between(SkipWhitespaces)
                .Select(x => new BoundNode(ctx => nodeFactory.CreateLiteralValueNode(x)))
                .Labelled("long-integer-literal");

            var doubleLiteral = Tokens.DoubleToken
                .Between(SkipWhitespaces)
                .Select(x => new BoundNode(ctx => nodeFactory.CreateLiteralValueNode(x)))
                .Labelled("double-literal");

            var floatLiteral = Tokens.FloatToken
                .Between(SkipWhitespaces)
                .Select(x => new BoundNode(ctx => nodeFactory.CreateLiteralValueNode(x)))
                .Labelled("float-literal");

            var decimalLiteral = Tokens.DecimalToken
                .Between(SkipWhitespaces)
                .Select(x => new BoundNode(ctx => nodeFactory.CreateLiteralValueNode(x)))
                .Labelled("decimal-literal");

            var numberLiteral = OneOf(
                Try(decimalLiteral),
                Try(floatLiteral),
                Try(doubleLiteral),
                Try(longIntegerLiteral),
                Try(integerLiteral)
            );

            var stringLiteral = Tokens.StringToken
                .Labelled("string literal")
                .Select(x => new BoundNode(ctx => nodeFactory.CreateLiteralValueNode(x)));

            var guidLiteral = Tokens.GuidToken
                .Between(SkipWhitespaces)
                .Select(x => new BoundNode(ctx => nodeFactory.CreateLiteralValueNode(x)))
                .Labelled("guid-literal");

            var dateTimeLiteral = Tokens.DateTimeToken
                .Between(SkipWhitespaces)
                .Select(x => new BoundNode(ctx => nodeFactory.CreateLiteralValueNode(x)))
                .Labelled("datetime-literal");

            var dateTimeOffsetLiteral = Tokens.DateTimeOffsetToken
                .Between(SkipWhitespaces)
                .Select(x => new BoundNode(ctx => nodeFactory.CreateLiteralValueNode(x)))
                .Labelled("datetimeoffset-literal");

            var instantLiteral = OneOf(Try(dateTimeOffsetLiteral), Try(dateTimeLiteral));

            var builtinLiteral = OneOf(
                Try(nullLiteral),
                Try(booleanLiteralParser),
                Try(stringLiteral),
                Try(guidLiteral),
                instantLiteral,
                numberLiteral
            );

            var customLiterals = _customLiteralProvider?
                .GetLiteralParsers<TNode>()
                .Select(parser => parser.Select(lit => new BoundNode(ctx => nodeFactory.CreateCustomLiteralValueNode(lit))))
                ?? new Parser<char, BoundNode>[] { };

            var customLiteral =
                OneOf(customLiterals.Select(x => Try(x)))
                .Labelled("custom-literal");

            var literal = (builtinLiteral.Or(customLiteral)).Between(SkipWhitespaces).Labelled("literal");

            var constant = literal;

            var identifier = Tokens.IdentifierToken
                .Labelled("identifier");

            var contextSymbolAccess = identifier
                .Between(SkipWhitespaces)
                .Select(variableName => new BoundNode(ctx => nodeFactory.CreateSymbolAccessNode(ctx, variableName)))
                .Labelled("context-symbol-access");

            //TODO 
            var navigationMemberAccess =
                    Map((variable, dot, members) => new BoundNode(ctx => nodeFactory.CreateMemberAccessNode(variable.Invoke(ctx), members)),
                    contextSymbolAccess,
                    Tokens.Dot,
                    identifier.Between(SkipWhitespaces).Separated(Tokens.Dot)
                ).Labelled("navigation-member-access");

            var symbol = Tokens.SymbolToken.Labelled("symbol");

            var listEvaluation = default(Parser<char, BoundNode>);

            var vector = default(Parser<char, BoundNode>);

            var expression = OneOf(Try(constant), Try(Rec(() => listEvaluation)), Try(Rec(() => vector)), Try(navigationMemberAccess))
                .Between(SkipWhitespaces)
                .Labelled("expression");

            var vectorBody = expression.Between(SkipWhitespaces)
                .Separated(Tokens.Comma)
                .Labelled("vector-body");

            vector = vectorBody
                .Between(Tokens.LBracket, Tokens.RBracket)
                .Select(elements => new BoundNode(ctx => nodeFactory.CreateVectorNode(elements.Select(e => e.Invoke(ctx)))))
                .Labelled("vector");

            var listBody = expression.Between(SkipWhitespaces)
                .Separated(Tokens.Comma)
                .Labelled("list-body");

            listEvaluation = Map(
                (sym, elements) => new BoundNode(ctx => nodeFactory.CreateListEvaluationNode(ctx, sym, elements.Select(e => e.Invoke(ctx)))),
                symbol.Between(SkipWhitespaces),
                listBody.Between(Tokens.LParenthese, Tokens.RParenthese)
            ).Labelled("list-evaluation");

            this.UnboundRootParser = listEvaluation;
        }

        public TNode Parse(string input, ParseContext<TNode> context)
        {
            if (this.TryParse(input, out var result, context))
            {
                return result;
            }
            else
            {
                throw new ArgumentException(nameof(context));
            }
        }

        public bool TryParse(string input, out TNode result, ParseContext<TNode> context)
        {
            if(context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            var unboundResult = this.UnboundRootParser.Parse(input);
            if (unboundResult.Success)
            {
                result = unboundResult.Value.Invoke(context);
                return true;
            }
            else
            {
                result = default;
                return false;
            }
        }

        private static string MakeString(IEnumerable<char> chars) =>
            string.Concat(chars);

    }
}
