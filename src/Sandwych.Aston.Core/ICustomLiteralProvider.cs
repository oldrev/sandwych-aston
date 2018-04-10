using System;
using System.Collections.Generic;
using System.Text;
using Pidgin;
using static Pidgin.Parser;
using static Pidgin.Parser<char>;

namespace Sandwych.Aston
{
    public interface ICustomLiteralProvider<TNode>
    {
        IEnumerable<Parser<char, TNode>> GetLiteralParsers();
    }
}
