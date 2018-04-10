using System;
using System.Collections.Generic;
using System.Text;

namespace Sandwych.Aston
{
    public interface INodeClrTypeEvaluator<TNode>
    {
        Type Evaluate(TNode node);
    }
}
