namespace QParser;

public struct CharPosition
{
    public int Line = -1;
    public int Column = -1;
    public readonly FileInformation FileInformation;
    public static int TabWidth = 4;
    public static readonly CharPosition Undefined = new();

    public CharPosition(int line, int column, FileInformation fileInformation)
    {
        Line = line;
        Column = column;
        FileInformation = fileInformation;
    }

    public override string ToString()
    {
        return $"{Line}:{Column}";
    }

    public string ToStringWithFile()
    {
        return $"{FileInformation}: {ToString()}";
    }

    public void Feed(char c)
    {
        if (c == '\n')
        {
            Line++;
            Column = 0;
        }
        // else if (c == '\t')
        // {
        //     Column += TabWidth;
        // }
        else
        {
            Column++;
        }
    }
}