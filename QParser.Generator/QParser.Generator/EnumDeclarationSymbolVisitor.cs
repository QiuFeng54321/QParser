using System.Collections.Generic;
using Microsoft.CodeAnalysis;

namespace QParser.Generator;

public class EnumDeclarationSymbolVisitor : SymbolVisitor
{
    public readonly Dictionary<string, int> Enums = new();

    public override void VisitField(IFieldSymbol symbol)
    {
        base.VisitField(symbol);
        if (symbol.ConstantValue is not int value) return;
        Enums.Add(symbol.Name, value);
    }
}