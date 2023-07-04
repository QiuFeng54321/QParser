namespace QParser;

public struct CharPosition
{
    
    public int Line;
    public int Column;
    public string FileName;
    public static int TabWidth = 4;

    public CharPosition(int line, int column, string fileName = "")
    {
        Line = line;
        Column = column;
        FileName = fileName;
    }

    public override string ToString()
    {
        return $"{Line}:{Column}";
    }

    public string ToStringWithFile()
    {
        return $"{FileName}: {ToString()}";
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