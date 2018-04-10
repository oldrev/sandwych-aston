using System;
using System.Collections.Generic;
using System.Text;

namespace Sandwych.Aston
{
    public interface IParseContext<TNode>
    {
        INodeFactory<TNode> NodeFactory { get; }
        IReadOnlyDictionary<string, Symbol> Symbols { get; }
        IReadOnlyDictionary<string, TNode> Parameters { get; }
    }
}
