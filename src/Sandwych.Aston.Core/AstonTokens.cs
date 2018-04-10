using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Pidgin;
using static Pidgin.Parser;
using static Pidgin.Parser<char>;

namespace Sandwych.Aston
{
    public static class AstonTokens
    {
        public static readonly Parser<char, Unit> SkipWhitespacesOrComment =
            SkipWhitespaces;

        public static readonly Parser<char, char> LBrace = Char('{');
        public static readonly Parser<char, char> RBrace = Char('}');
        public static readonly Parser<char, char> LBracket = Char('[');
        public static readonly Parser<char, char> RBracket = Char(']');
        public static readonly Parser<char, char> LParenthese = Char('(');
        public static readonly Parser<char, char> RParenthese = Char(')');
        public static readonly Parser<char, char> Quote = Char('"');
        public static readonly Parser<char, char> SingleQuote = Char('\'');
        public static readonly Parser<char, char> Colon = Char(':');
        public static readonly Parser<char, char> Comma = Char(',');
        public static readonly Parser<char, char> Plus = Char('+');
        public static readonly Parser<char, char> Minus = Char('-');
        public static readonly Parser<char, char> Dot = Char('.');

        public static readonly Parser<char, char> DoubleQuotedEscapeCharacter =
            OneOf('"', 'n', 'r', 't', '\\')
            .Labelled("double-quoted-escape-character");

        public static readonly Parser<char, char> SingleQuotedEscapeCharacter =
            OneOf('\'', 'n', 'r', 't', '\\')
            .Labelled("single-quoted-escape-character");

        public static readonly Parser<char, string> IdentifierToken = Token(c => c == '_' || char.IsLetter(c))
            .Then(Token(char.IsLetterOrDigit).Many(), MakeString)
            .Labelled("identifier");

        public static readonly Parser<char, char> AllSymbolCharacter =
            Token(c => char.IsLetterOrDigit(c) ||
            c == '_' || c == '+' || c == '-' || c == '*' || c == '/' || c == '^' || c == '&' || c == '%' || c == '>' || c == '<' || c == '=');

        public static readonly Parser<char, char> FirstSymbolCharacter =
            Token(c => char.IsLetter(c) ||
            c == '_' || c == '+' || c == '-' || c == '*' || c == '/' || c == '^' || c == '&' || c == '%' || c == '>' || c == '<' || c == '=');

        public static readonly Parser<char, string> SymbolToken = FirstSymbolCharacter
                .Then(AllSymbolCharacter.Many(), MakeString);

        public static readonly Parser<char, string> DoubleQuotedStringToken =
            Char('\\').Then(DoubleQuotedEscapeCharacter).Or(Token(c => c != '"'))
            .ManyString()
            .Between(Quote);

        public static readonly Parser<char, string> SingleQuotedStringToken =
            Char('\\').Then(SingleQuotedEscapeCharacter).Or(Token(c => c != '\''))
            .ManyString()
            .Between(SingleQuote);

        public static readonly Parser<char, string> StringToken = SingleQuotedStringToken.Or(DoubleQuotedStringToken);

        public static readonly Parser<char, int> IntegerToken = DecimalNum.Between(Whitespaces);

        public static readonly Parser<char, char> LongIntegerSuffix = Char('L').Or(Char('l'));

        public static readonly Parser<char, long> LongIntegerToken =
            Map((num, suffix) => num, LongNum, LongIntegerSuffix);

        public static readonly Parser<char, IEnumerable<char>> RealNumberFractionalPart =
            Map(MakeCharSeq, Char('.'), Digit.AtLeastOnce());

        public static readonly Parser<char, IEnumerable<char>> RealNumberIntegerPart = Digit.AtLeastOnce();

        public static readonly Parser<char, IEnumerable<char>> RealNumber =
            Map((sign, i, f) => sign.HasValue ? MakeCharSeq(sign.Value, i.Concat(f)) : i.Concat(f),
            (Char('+').Or(Char('-'))).Optional(),
            RealNumberIntegerPart,
            RealNumberFractionalPart);

        public static readonly Parser<char, double> DoubleToken =
            Map((num, suffix) => double.Parse(string.Concat(num)),
                RealNumber,
                (Char('D').Or(Char('d'))).Optional());

        public static readonly Parser<char, float> FloatToken =
            Map((num, suffix) => float.Parse(string.Concat(num)),
                RealNumber,
                Char('F').Or(Char('f')));

        public static readonly Parser<char, decimal> DecimalToken =
            Map((num, suffix) => decimal.Parse(string.Concat(num)),
                RealNumber,
                Char('M').Or(Char('m')));

        public static readonly Parser<char, Guid> UuidToken =
            Map((prefix, uuid) => Guid.Parse(string.Concat(uuid)),
                String("uuid"),
                StringToken);

        private static IEnumerable<char> MakeCharSeq(char first, IEnumerable<char> rest)
        {
            yield return first;
            foreach (var c in rest)
            {
                yield return c;
            }
        }

        private static string MakeString(char first, IEnumerable<char> rest)
        {
            var sb = new StringBuilder();
            sb.Append(first);
            sb.Append(string.Concat(rest));
            return sb.ToString();
        }

    }
}
