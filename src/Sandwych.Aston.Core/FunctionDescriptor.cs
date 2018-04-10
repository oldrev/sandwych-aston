using System;
using System.Collections.Generic;
using System.Text;

namespace Sandwych.Aston
{
    public class FunctionDescriptor<TNode>
    {
        public string Name { get; set; }
        public bool IsParameterVariadic { get; set; }
        public int ParametersCount { get; set; }
        public Func<IEnumerable<TNode>, TNode> InvocationFactory { get; set; }
    }
}
