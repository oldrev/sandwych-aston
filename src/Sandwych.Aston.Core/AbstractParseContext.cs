using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Sandwych.Aston
{
    public abstract class AbstractParseContext<TNode> : IParseContext<TNode>
    {
        private readonly Dictionary<string, Symbol> _symbols = new Dictionary<string, Symbol>();
        private readonly Dictionary<string, TNode> _parameters = new Dictionary<string, TNode>();

        public IReadOnlyDictionary<string, Symbol> Symbols => _symbols;

        public INodeFactory<TNode> NodeFactory { get; }

        public IReadOnlyDictionary<string, TNode> Parameters => _parameters;

        public AbstractParseContext(INodeFactory<TNode> nodeFactory, IEnumerable<Symbol> symbols = null)
        {
            this.NodeFactory = nodeFactory ?? throw new ArgumentNullException(nameof(nodeFactory));

            if (symbols != null)
            {
                foreach (var symbol in symbols)
                {
                    _symbols.Add(symbol.Name, symbol);
                }
            }
        }

        public AbstractParseContext(INodeFactory<TNode> nodeFactory, Type singleParameterType, IEnumerable<Symbol> additionalSymbols = null)
        {
            this.NodeFactory = nodeFactory ?? throw new ArgumentNullException(nameof(nodeFactory));
            if (additionalSymbols != null)
            {
                foreach (var symbol in additionalSymbols)
                {
                    _symbols.Add(symbol.Name, symbol);
                }
            }
        }

        public void RegisterParameter(Type type, string symbol)
        {
            _symbols.Add(symbol, new Symbol(symbol, SymbolType.Parameter, type));
            var parameterExpr = this.NodeFactory.CreateParameterNode(type, symbol);
            _parameters.Add(symbol, parameterExpr);
        }

    }
}
