using System;
using System.Collections.Generic;
using System.Text;

namespace Sandwych.Aston
{
    public interface IBuiltinFunctionStrategy<TNode>
    {
        void RegisterBuiltinFunctions(ParseContext<TNode> context);
    }
}
