using System.IO;
using System.Text;

namespace QParser;

public class SourceInputStream : StreamReader
{
    private readonly StringBuilder _currentLineBuilder = new();
    public readonly FileInformation FileInformation;

    public SourceInputStream(FileInformation fileInformation, Stream stream) : base(stream, Encoding.UTF8)
    {
        FileInformation = fileInformation;
    }

    public override void Close()
    {
        base.Close();
        FinishLineRecording();
    }

    public void FinishLineRecording()
    {
        if (_currentLineBuilder.Length <= 0) return;
        FileInformation.Lines.Add(_currentLineBuilder.ToString());
        _currentLineBuilder.Clear();
    }


    public override int Read()
    {
        var c = base.Read();
        if (c == '\n')
        {
            FileInformation.Lines.Add(_currentLineBuilder.ToString());
            _currentLineBuilder.Clear();
        }
        else
        {
            _currentLineBuilder.Append((char)c);
        }

        return c;
    }
}