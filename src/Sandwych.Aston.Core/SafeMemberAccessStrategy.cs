using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace Sandwych.Aston.Linq.Expressions;

public class SafeMemberAccessStrategy<TNode> : IMemberAccessStrategy
{
    private HashSet<string> _allowedMembers = new HashSet<string>();

    private readonly IMemberAccessStrategy _parent;
    private readonly INodeClrTypeEvaluator<TNode> _clrTypeEvaluator;

    public SafeMemberAccessStrategy(INodeClrTypeEvaluator<TNode> clrTypeEvaluator)
    {
        _clrTypeEvaluator = clrTypeEvaluator ?? throw new ArgumentNullException(nameof(clrTypeEvaluator));
    }

    public SafeMemberAccessStrategy(
        IMemberAccessStrategy parent,
        INodeFactory<TNode> nodeFactory,
        INodeClrTypeEvaluator<TNode> clrTypeEvaluator)
        : this(clrTypeEvaluator)
    {
        _parent = parent;
    }

    public bool IsAllowed(object objExpr, string name)
    {
        var objType = _clrTypeEvaluator.Evaluate((TNode)objExpr);
        if (_allowedMembers.Count > 0)
        {
            while (objType != null)
            {
                // Look for specific property map
                if (_allowedMembers.Contains(Key(objType, name)))
                {
                    return true;
                }

                // Look for a catch-all getter
                if (_allowedMembers.Contains(Key(objType, "*")))
                {
                    return true;
                }

                objType = objType.GetTypeInfo().BaseType;
            }
        }

        return _parent?.IsAllowed(objExpr, name) ?? false;
    }

    public void Register(Type type, string name)
    {
        _allowedMembers.Add(this.Key(type, name));
    }

    private string Key(Type type, string name) => $"{type.Name}.{name}";

}
