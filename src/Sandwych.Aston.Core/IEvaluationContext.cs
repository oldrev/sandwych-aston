using System;
using System.Collections.Generic;
using System.Text;

namespace Sandwych.Aston;

public interface IEvaluationContext
{
    IReadOnlyDictionary<string, object> Variables { get; }
}
