using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Sandwych.Aston
{
    public class ParseContext<TNode>
    {
        private readonly Dictionary<string, Symbol> _symbols = new Dictionary<string, Symbol>();
        private readonly Dictionary<string, TNode> _parameters = new Dictionary<string, TNode>();
        private readonly Dictionary<string, FunctionDescriptor<TNode>> _functions = new Dictionary<string, FunctionDescriptor<TNode>>();

        public IReadOnlyDictionary<string, Symbol> Symbols => _symbols;

        public IReadOnlyDictionary<string, FunctionDescriptor<TNode>> Functions => _functions;

        public IReadOnlyDictionary<string, TNode> Parameters => _parameters;

        public ParseContext(IEnumerable<Symbol> symbols = null)
        {
            if (symbols != null)
            {
                foreach (var symbol in symbols)
                {
                    _symbols.Add(symbol.Name, symbol);
                }
            }
        }

        public ParseContext(Type singleParameterType, IEnumerable<Symbol> additionalSymbols = null)
        {
            if (additionalSymbols != null)
            {
                foreach (var symbol in additionalSymbols)
                {
                    _symbols.Add(symbol.Name, symbol);
                }
            }
        }

        public void RegisterParameter(INodeFactory<TNode> nodeFactory, Type type, string symbol)
        {
            _symbols.Add(symbol, new Symbol(symbol, SymbolType.Parameter, type));
            var parameterNode = nodeFactory.CreateParameterNode(type, symbol);
            _parameters.Add(symbol, parameterNode);
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
            string name, bool isParameterVariadic, int parametersCount, Func<IEnumerable<TNode>, TNode> invocationFactory)
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

    }
}
