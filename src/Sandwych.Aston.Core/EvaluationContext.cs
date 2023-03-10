using System;
using System.Collections.Generic;
using System.Text;

namespace Sandwych.Aston;

public class EvaluationContext : IEvaluationContext
{
    public static readonly EvaluationContext Empty = new EvaluationContext();

    public IReadOnlyDictionary<string, object> Variables { get; }

    public EvaluationContext(IReadOnlyDictionary<string, object> variables = null)
    {
        this.Variables = variables ?? new Dictionary<string, object>(0);
    }
}
