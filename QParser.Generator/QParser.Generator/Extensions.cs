using System;
using System.Linq;
using System.Text.RegularExpressions;
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
}