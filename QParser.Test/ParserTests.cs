using System.Collections;
using NUnit.Framework;
using QParser.Lexer;
using QParser.Parser;
using QParser.Test;
using QParser.Test.TestParsers;

namespace QParser.Test;
public class ParserTestsFixtureData
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

[TestFixtureSource(typeof(ParserTestsFixtureData), nameof(ParserTestsFixtureData.FixtureParams))]
public class ParserTests
{
    private readonly GrammarConstructor _grammarConstructor;

    public ParserTests(GrammarConstructor grammarConstructor)
    {
        _grammarConstructor = grammarConstructor;
    }

    [SetUp]
    public void Setup()
    {
        _grammarConstructor.Construct();
        _grammarConstructor.Grammar.GenerateCanBeEmpty();
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