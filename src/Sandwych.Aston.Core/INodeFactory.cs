using System;
using System.Collections.Generic;
using System.Text;

namespace Sandwych.Aston
{
    public interface INodeFactory<TNode>
    {
        IMemberAccessStrategy<TNode> MemberAccessStrategy { get; }

        TNode CreateNullLiteralNode();
        TNode CreateBooleanLiteralNode(bool value);
        TNode CreateStringLiteralNode(string value);
        TNode CreateIntegerLiteralNode(int value);
        TNode CreateLongIntegerLiteralNode(long value);
        TNode CreateDoubleLiteralNode(double value);
        TNode CreateFloatLiteralNode(float value);
        TNode CreateDecimalLiteralNode(decimal value);
        TNode CreateGuidLiteralNode(Guid value);
        TNode CreateDateTimeLiteralNode(DateTime value);
        TNode CreateDateTimeOffsetLiteralNode(DateTimeOffset value);
        TNode CreateVectorNode(IEnumerable<TNode> items);
        TNode CreateListEvaluationNode(IParseContext<TNode> context, string symbol, IEnumerable<TNode> elements);
        TNode CreateCustomLiteralNode(object value);
        TNode CreateMemberAccessNode(TNode objExpr, IEnumerable<string> memberPath);
        TNode CreateParameterNode(Type type, string name);
        TNode CreateSymbolAccessNode(IParseContext<TNode> context, string variable);
    }
}
