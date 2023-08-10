using System;
using System.Collections.Generic;

namespace QParser;

public class FileInformation
{
    public readonly List<PrettyException> Exceptions = new();
    public readonly string FilePath;
    public readonly List<string> Lines = new();

    public FileInformation(string filePath)
    {
        FilePath = filePath;
    }

    public override string ToString()
    {
        return FilePath;
    }

    public void DumpExceptions()
    {
        foreach (var exception in Exceptions) Console.WriteLine(exception.ToString());
    }
}