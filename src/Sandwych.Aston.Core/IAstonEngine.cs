using System;
using System.Collections.Generic;
using System.Text;

namespace Sandwych.Aston;

public interface IAstonEngine<TNode>
{
    AstonParser<TNode> Parser { get; }

    bool TryParse(string input, out TNode astRootNode, ParseContext<TNode> context = null);
}
