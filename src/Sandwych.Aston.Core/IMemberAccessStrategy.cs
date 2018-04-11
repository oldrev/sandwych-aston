using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Sandwych.Aston
{
    public interface IMemberAccessStrategy
    {
        bool IsAllowed(object objExprNode, string memberName);

        void Register(Type type, string name);
    }
}
