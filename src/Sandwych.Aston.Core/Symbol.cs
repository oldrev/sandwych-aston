using System;
using System.Collections.Generic;
using System.Text;

namespace Sandwych.Aston;

public readonly struct Symbol
{
    public string Name { get; }
    public SymbolType Type { get; }
    public Type ClrType { get; }

    public Symbol(string name, SymbolType type, Type clrType)
    {
        this.Name = name;
        this.Type = type;
        this.ClrType = clrType;
    }

    public override int GetHashCode() =>
        this.Name.GetHashCode();

}
