using System;
using System.Collections.Generic;
using System.Text;

namespace Sandwych.Aston;

public abstract class AbstractAstonEngine<TNode> : IAstonEngine<TNode>
{
    public AstonParser<TNode> Parser { get; }

    protected abstract INodeFactory<TNode> NodeFactory { get; }

    public AbstractAstonEngine(Options options)
    {
        this.Parser = new AstonParser<TNode>(this.NodeFactory, options.CustomLiteralProvider);
    }

    public bool TryParse(string input, out TNode astRootNode, ParseContext<TNode> context = null)
    {
        return this.Parser.TryParse(input, out astRootNode, context);
    }
}
