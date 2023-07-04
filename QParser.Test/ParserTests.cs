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
    }

    [Test]
    public void Test()
    {
        Console.WriteLine(_grammarConstructor.Grammar.ToString());
    }
}