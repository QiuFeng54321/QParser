using System.Text;

namespace QParser;

public class PrettyException
{
    public readonly FileInformation FileInformation;
    public readonly string Message;
    public readonly SourceRange SourceRange;

    public PrettyException(FileInformation fileInformation, SourceRange sourceRange, string message)
    {
        SourceRange = sourceRange;
        Message = message;
        FileInformation = fileInformation;
    }

    public override string ToString()
    {
        StringBuilder sb = new();
        sb.Append(SourceRange.ToStringWithFile())
            .Append(": ")
            .Append(GetType().Name)
            .Append(": ")
            .Append(Message)
            .AppendLine(":");
        const int maxLineBefore = 3;
        var linesBefore = int.Min(SourceRange.Start.Line, maxLineBefore);
        for (var i = 0; i < linesBefore; i++)
            sb.AppendLine(FileInformation.Lines[SourceRange.Start.Line - linesBefore + i]);

        Console.WriteLine($"{SourceRange.Start} .. {SourceRange.End}");
        for (var line = SourceRange.Start.Line; line <= SourceRange.End.Line; line++)
        {
            sb.AppendLine(FileInformation.Lines[line]);
            var spacesBefore = line == SourceRange.Start.Line ? SourceRange.Start.Column : 0;
            var spacesAfter = line == SourceRange.End.Line
                ? FileInformation.Lines[line].Length - SourceRange.End.Column - 1
                : 0;
            var tildeCount = FileInformation.Lines[line].Length - spacesBefore - spacesAfter;
            sb.Append(new string(' ', spacesBefore));
            sb.Append(new string('~', tildeCount));
            sb.AppendLine(new string(' ', spacesAfter));
        }

        return sb.ToString();
    }

    public void AddToExceptions()
    {
        FileInformation.Exceptions.Add(this);
    }
}