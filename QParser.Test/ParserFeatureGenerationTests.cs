using System.Collections;
using QParser.Parser;
using QParser.Parser.LR;
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
            yield return new TestFixtureData(() => new DragonBookEg0455GrammarConstructor());
            yield return new TestFixtureData(() => new BNF_GrammarConstructor());
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
    public void GenerateLR0Closures()
    {
        if (_grammar.EntryRule == null) Assert.Pass();
        _grammar.GenerateNonKernelItems();
        var closureTable = new LR0ClosureTable(_grammar.EntryRule);
        closureTable.Generate();
        closureTable.Dump();
    }

    [Test]
    public void GenerateLR1Closures()
    {
        if (_grammar.EntryRule == null) Assert.Pass();
        _grammar.GenerateNonKernelItems();
        var closureTable = new LR1ClosureTable(_grammar.EntryRule);
        closureTable.Generate();
        closureTable.Dump();
    }

    [Test]
    public void GenerateSLRParser()
    {
        if (_grammar.EntryRule == null) Assert.Pass();
        _grammar.GenerateAll();
        var parser = new SLRParser(_grammar, null!);
        var isSLR = parser.GenerateTables();
        parser.DumpTables();
        Console.WriteLine($"Is SLR: {isSLR}");
    }

    [Test]
    public void GenerateLRParser()
    {
        if (_grammar.EntryRule == null) Assert.Pass();
        _grammar.GenerateAll();
        var parser = new CanonicalLRParser(_grammar, null!);
        var isLR = parser.GenerateTables();
        parser.DumpTables();
        Console.WriteLine($"Is LR: {isLR}");
    }
}