using System.Collections;
using QParser.Parser;
using QParser.Test.TestParsers;

namespace QParser.Test;

public class ParserFeatureGenerationTestsFixtureData
{
    public static IEnumerable FixtureParams
    {
        get
        {
            yield return new TestFixtureData(new TestParser1GrammarConstructor());
            yield return new TestFixtureData(new TestParser2GrammarConstructor());
            yield return new TestFixtureData(new TestParser3RecursiveGrammarConstructor());
            yield return new TestFixtureData(new DragonBook4_2_1GrammarConstructor());
            yield return new TestFixtureData(new DragonBook4_2_2eGrammarConstructor());
            yield return new TestFixtureData(new DragonBook4_2_2fGrammarConstructor());
        }
    }
}

[TestFixtureSource(typeof(ParserFeatureGenerationTestsFixtureData),
    nameof(ParserFeatureGenerationTestsFixtureData.FixtureParams))]
public class ParserFeatureGenerationTests
{
    [SetUp]
    public void Setup()
    {
        _grammarConstructor.Construct();
        _grammarConstructor.Grammar.GenerateCanBeEmpty();
    }

    private readonly GrammarConstructor _grammarConstructor;

    public ParserFeatureGenerationTests(GrammarConstructor grammarConstructor)
    {
        _grammarConstructor = grammarConstructor;
    }

    [Test]
    public void OutputGrammar()
    {
        Console.WriteLine("Original ------------------------------");
        Console.WriteLine(_grammarConstructor.Grammar);
    }

    [Test]
    public void GenerateFirstAndFollow()
    {
        Console.WriteLine("First ------------------------------");
        _grammarConstructor.Grammar.GenerateFirst();
        Console.WriteLine(_grammarConstructor.Grammar.ToString(true, false));

        Console.WriteLine("Follow ------------------------------");
        _grammarConstructor.Grammar.GenerateFollow();
        Console.WriteLine(_grammarConstructor.Grammar.ToString(false, true));
    }
}