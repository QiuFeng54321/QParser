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
            yield return new TestFixtureData(() => new TestParser1GrammarConstructor());
            yield return new TestFixtureData(() => new TestParser2GrammarConstructor());
            yield return new TestFixtureData(() => new TestParser3RecursiveGrammarConstructor());
            yield return new TestFixtureData(() => new DragonBook4_2_1GrammarConstructor());
            yield return new TestFixtureData(() => new DragonBook4_2_2eGrammarConstructor());
            yield return new TestFixtureData(() => new DragonBook4_2_2fGrammarConstructor());
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
        var grammarConstructor = _grammarConstructorFactory();
        grammarConstructor.Construct();
        grammarConstructor.Grammar.GenerateCanBeEmpty();
        _grammar = grammarConstructor.Grammar;
    }

    private Grammar _grammar = null!;
    private readonly Func<GrammarConstructor> _grammarConstructorFactory;

    public ParserFeatureGenerationTests(Func<GrammarConstructor> grammarConstructorFactory)
    {
        _grammarConstructorFactory = grammarConstructorFactory;
    }

    [Test]
    public void OutputGrammar()
    {
        Console.WriteLine("Original ------------------------------");
        Console.WriteLine(_grammar.ToString(false, false));
    }

    [Test]
    public void GenerateFirstAndFollow()
    {
        Console.WriteLine("First ------------------------------");
        _grammar.GenerateFirst();
        Console.WriteLine(_grammar.ToString(true, false));

        Console.WriteLine("Follow ------------------------------");
        _grammar.GenerateFollow();
        Console.WriteLine(_grammar.ToString(false, true));
    }

    [Test]
    public void GenerateNonKernelItems()
    {
        _grammar.GenerateNonKernelItems();
        foreach (var rule in _grammar.Rules)
        {
            Console.WriteLine($"Non-kernel item for {rule}: ");
            foreach (var nonKernelItem in rule.NonKernelItems) Console.WriteLine(nonKernelItem);
        }
    }

    [Test]
    public void GenerateClosures()
    {
        if (_grammar.EntryRule == null) Assert.Pass();
        _grammar.GenerateNonKernelItems();
        var closureTable = new ClosureTable(_grammar.EntryRule);
        closureTable.Generate();
        closureTable.Dump();
    }

    [Test]
    public void GenerateSLRParser()
    {
        if (_grammar.EntryRule == null) Assert.Pass();
        _grammar.GenerateAll();
        var parser = new SLRParser(_grammar);
        var isSLR = parser.GenerateTables();
        parser.Dump();
        Console.WriteLine($"Is SLR: {isSLR}");
    }
}