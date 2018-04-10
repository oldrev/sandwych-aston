using System;
using System.Collections.Generic;
using System.Text;

namespace Sandwych.Aston
{
    public interface IExpressionEngine<TNode>
    {
        bool TryParse(string input, out TNode astRootNode);
    }
}
