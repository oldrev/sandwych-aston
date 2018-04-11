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
        private readonly INodeFactory<TNode> _nodeFactory;
        private readonly ICustomLiteralProvider<TNode> _customLiteralProvider;
        private readonly IParseContext<TNode> _parseContext;

        public Parser<char, TNode> RootParser { get; private set; }

        public AstonParser(IParseContext<TNode> context, ICustomLiteralProvider<TNode> customLiteralProvider = null)
        {
            _parseContext = context ?? throw new ArgumentNullException(nameof(context));
            _nodeFactory = context.NodeFactory;
            _customLiteralProvider = customLiteralProvider;

            // Literals:
            var nullLiteral = String("null").Select(x => _nodeFactory.CreateNullLiteralNode());

            var trueLiteralParser = String("true").Select(x => true);
            var falseLiteralParser = String("false").Select(x => false);
            var booleanLiteralParser = trueLiteralParser.Or(falseLiteralParser).Select(x => _nodeFactory.CreateBooleanLiteralNode(x));

            var integerLiteral =
                Tokens.IntegerToken
                .Between(SkipWhitespaces)
                .Select(x => _nodeFactory.CreateIntegerLiteralNode(x))
                .Labelled("integer-literal");

            var longIntegerLiteral = Tokens.LongIntegerToken
                .Between(SkipWhitespaces)
                .Select(x => _nodeFactory.CreateLongIntegerLiteralNode(x))
                .Labelled("long-integer-literal");

            var doubleLiteral = Tokens.DoubleToken
                .Between(SkipWhitespaces)
                .Select(x => _nodeFactory.CreateDoubleLiteralNode(x))
                .Labelled("double-literal");

            var floatLiteral = Tokens.FloatToken
                .Between(SkipWhitespaces)
                .Select(x => _nodeFactory.CreateFloatLiteralNode(x))
                .Labelled("float-literal");

            var decimalLiteral = Tokens.DecimalToken
                .Between(SkipWhitespaces)
                .Select(x => _nodeFactory.CreateDecimalLiteralNode(x))
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
                .Select(x => _nodeFactory.CreateStringLiteralNode(x));

            var guidLiteral = Tokens.GuidToken
                .Between(SkipWhitespaces)
                .Select(x => _nodeFactory.CreateGuidLiteralNode(x))
                .Labelled("guid-literal");

            var dateTimeLiteral = Tokens.DateTimeToken
                .Between(SkipWhitespaces)
                .Select(x => _nodeFactory.CreateDateTimeLiteralNode(x))
                .Labelled("datetime-literal");

            var dateTimeOffsetLiteral = Tokens.DateTimeOffsetToken
                .Between(SkipWhitespaces)
                .Select(x => _nodeFactory.CreateDateTimeOffsetLiteralNode(x))
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

            var customLiterals = customLiteralProvider?.GetLiteralParsers() ?? new Parser<char, TNode>[] { };

            var customLiteral =
                OneOf(customLiterals.Select(x => Try(x)))
                .Labelled("custom-literal");

            var literal = (builtinLiteral.Or(customLiteral)).Between(SkipWhitespaces).Labelled("literal");

            var constant = literal;

            var identifier = Tokens.IdentifierToken
                .Labelled("identifier");

            var contextSymbolAccess = identifier
                .Between(SkipWhitespaces)
                .Select(variableName => _nodeFactory.CreateSymbolAccessNode(_parseContext, variableName))
                .Labelled("context-symbol-access");

            //TODO 
            var navigationMemberAccess =
                    Map((variable, dot, members) => _nodeFactory.CreateMemberAccessNode(variable, members),
                    contextSymbolAccess,
                    Tokens.Dot,
                    identifier.Between(SkipWhitespaces).Separated(Tokens.Dot)
                ).Labelled("navigation-member-access");

            var symbol = Tokens.SymbolToken.Labelled("symbol");

            var listEvaluation = default(Parser<char, TNode>);

            var vector = default(Parser<char, TNode>);

            var expression = OneOf(Try(constant), Try(Rec(() => listEvaluation)), Try(Rec(() => vector)), Try(navigationMemberAccess))
                .Between(SkipWhitespaces)
                .Labelled("expression");

            var vectorBody = expression.Between(SkipWhitespaces)
                .Separated(Tokens.Comma)
                .Labelled("vector-body");

            vector = vectorBody
                .Between(Tokens.LBracket, Tokens.RBracket)
                .Select(x => _nodeFactory.CreateVectorNode(x))
                .Labelled("vector");

            var listBody = expression.Between(SkipWhitespaces)
                .Separated(Tokens.Comma)
                .Labelled("list-body");

            listEvaluation = Map(
                (sym, elements) => _nodeFactory.CreateListEvaluationNode(context, sym, elements),
                symbol.Between(SkipWhitespaces),
                listBody.Between(Tokens.LParenthese, Tokens.RParenthese)
            ).Labelled("list-evaluation");

            this.RootParser = listEvaluation;
        }

        public Result<char, TNode> Parse(string input) =>
            this.RootParser.Parse(input);

        private static string MakeString(IEnumerable<char> chars) =>
            string.Concat(chars);

        private IEnumerable<Parser<char, TNode>> GetCustomLiteralParsers()
        {
            var parsers = _customLiteralProvider?.GetLiteralParsers() ?? new Parser<char, TNode>[] { };
            foreach (var lp in parsers)
            {
                yield return Try(lp).Select(x => _nodeFactory.CreateCustomLiteralNode(x));
            }
        }

    }
}
