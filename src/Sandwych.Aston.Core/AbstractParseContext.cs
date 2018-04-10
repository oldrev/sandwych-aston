using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Sandwych.Aston
{
    public abstract class AbstractParseContext<TNode> : IParseContext<TNode>
    {
        private readonly Dictionary<string, Symbol> _symbols = new Dictionary<string, Symbol>();
        private readonly Dictionary<string, TNode> _parameters = new Dictionary<string, TNode>();
        private readonly Dictionary<string, FunctionDescriptor<TNode>> _functions = new Dictionary<string, FunctionDescriptor<TNode>>();

        public IReadOnlyDictionary<string, Symbol> Symbols => _symbols;

        public IReadOnlyDictionary<string, FunctionDescriptor<TNode>> Functions => _functions;

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

            this.OnRegisterBuiltinFunctions();
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

            this.OnRegisterBuiltinFunctions();
        }

        public void RegisterParameter(Type type, string symbol)
        {
            _symbols.Add(symbol, new Symbol(symbol, SymbolType.Parameter, type));
            var parameterExpr = this.NodeFactory.CreateParameterNode(type, symbol);
            _parameters.Add(symbol, parameterExpr);
        }

        public void RegisterFunction(FunctionDescriptor<TNode> func)
        {
            if (_functions.ContainsKey(func.Name))
            {
                throw new ArgumentOutOfRangeException(nameof(func));
            }
            _functions.Add(func.Name, func);
        }

        public void RegisterFunction(
            string name, bool isParameterVariadic, int parametersCount,
            Func<IEnumerable<TNode>, TNode> invocationFactory)
        {
            var func = new FunctionDescriptor<TNode>()
            {
                Name = name,
                IsParameterVariadic = isParameterVariadic,
                ParametersCount = parametersCount,
                InvocationFactory = invocationFactory
            };
            this.RegisterFunction(func);
        }

        public void RegisterFunctions(IEnumerable<FunctionDescriptor<TNode>> functions)
        {
            foreach (var func in functions)
            {
                this.RegisterFunction(func);
            }
        }

        protected abstract void OnRegisterBuiltinFunctions();

    }
}
