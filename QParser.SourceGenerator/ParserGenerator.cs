using Microsoft.CodeAnalysis;

namespace QParser.SourceGenerator;

[Generator]
public class ParserGenerator : ISourceGenerator
{
    public void Initialize(GeneratorInitializationContext context)
    {
    }

    public void Execute(GeneratorExecutionContext context)
    {
        var grammarFiles = context.AdditionalFiles.Where(f => f.Path.EndsWith(".qg")).ToList();
    }
}