namespace QParser;

public class SourceRange
{
    public CharPosition End;
    public CharPosition Start;

    public SourceRange(CharPosition start, CharPosition end)
    {
        Start = start;
        End = end;
    }

    public override string ToString()
    {
        return $"{Start} ~ {End}";
    }

    public string ToStringWithFile()
    {
        return $"{Start.FileInformation}: {ToString()}";
    }
}