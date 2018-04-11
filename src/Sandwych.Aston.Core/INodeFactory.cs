using System;
using System.Collections.Generic;
using System.Text;

namespace Sandwych.Aston
{
    public interface INodeFactory<TNode>
    {
        IMemberAccessStrategy MemberAccessStrategy { get; }

        TNode CreateNullLiteralValueNode();
        TNode CreateLiteralValueNode(bool value);
        TNode CreateLiteralValueNode(string value);
        TNode CreateLiteralValueNode(int value);
        TNode CreateLiteralValueNode(long value);
        TNode CreateLiteralValueNode(double value);
        TNode CreateLiteralValueNode(float value);
        TNode CreateLiteralValueNode(decimal value);
        TNode CreateLiteralValueNode(Guid value);
        TNode CreateLiteralValueNode(DateTime value);
        TNode CreateLiteralValueNode(DateTimeOffset value);
        TNode CreateVectorNode(IEnumerable<TNode> items);
        TNode CreateListEvaluationNode(ParseContext<TNode> context, string symbol, IEnumerable<TNode> elements);
        TNode CreateCustomLiteralValueNode(object value);
        TNode CreateMemberAccessNode(TNode objExpr, IEnumerable<string> memberPath);
        TNode CreateParameterNode(Type type, string name);
        TNode CreateSymbolAccessNode(ParseContext<TNode> context, string variable);
    }
}
