using Microsoft.CodeAnalysis.Text;

namespace QParser.Generator;

public static class Extensions
{
    public static LinePositionSpan ToLinePositionSpan(this SourceRange sourceRange)
    {
        return new LinePositionSpan(new LinePosition(sourceRange.Start.Line, sourceRange.Start.Column),
            new LinePosition(sourceRange.End.Line, sourceRange.End.Column));
    }
}