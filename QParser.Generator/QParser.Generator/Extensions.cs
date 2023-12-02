using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace QParser.Generator;

public static class Extensions
{
    public static LinePositionSpan ToLinePositionSpan(this SourceRange sourceRange)
    {
        return new LinePositionSpan(new LinePosition(sourceRange.Start.Line, sourceRange.Start.Column),
            new LinePosition(sourceRange.End.Line, sourceRange.End.Column));
    }

    public static string StringExprToString(this string str)
    {
        return Regex.Unescape(str.Substring(1, str.Length - 2));
    }

    public static Type? FindType(this string typeName)
    {
        return AppDomain.CurrentDomain.GetAssemblies()
            .Reverse()
            .Select(assembly => assembly.GetType(typeName))
            .FirstOrDefault(t => t != null);
    }

    public static Dictionary<string, int> GetEnumsFromSyntax(this ITypeSymbol typeSymbol)
    {
        var members = typeSymbol.GetMembers();
        var visitor = new EnumDeclarationSymbolVisitor();
        foreach (var member in members) member.Accept(visitor);

        return visitor.Enums;
    }

    public static Dictionary<string, int> GetEnumsFromType(this Type type)
    {
        Dictionary<string, int> tokens = new();
        foreach (var tokenName in type.GetEnumNames())
        {
            var tokenValue = (int)Enum.Parse(type, tokenName);
            tokens[tokenName] = tokenValue;
        }

        return tokens;
    }
}