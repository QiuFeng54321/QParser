using System.Collections;
using System.Text;
using QParser.Lexer;
using QParser.Lexer.Tokens;

namespace QParser.Test;

public class LexerTestsFixtureData
{
    public static IEnumerable FixtureParams
    {
        get
        {
            yield return new TestFixtureData("Plain Symbols", new[]
            {
                "DECLARE",
                "    ",
                "O__$2932F",
                " ",
                "AS",
                ">=",
                "=",
                "<>",
                "AAA"
            }, new[]
            {
                DefaultTokenType.Declare,
                DefaultTokenType.Space,
                DefaultTokenType.Identifier,
                DefaultTokenType.Space,
                DefaultTokenType.As,
                DefaultTokenType.GreaterThanOrEqualTo,
                DefaultTokenType.Equal,
                DefaultTokenType.NotEqual,
                DefaultTokenType.Identifier
            });
            yield return new TestFixtureData("Last char token", new[]
                {
                    "a"
                },
                new[]
                {
                    DefaultTokenType.Identifier
                });
            yield return new TestFixtureData("Indent", new[]
            {
                " ",
                "AAA",
                "\n",
                "    ",
                "BBB",
                "    \n",
                " ",
                "C",
                "  "
            }, new[]
            {
                DefaultTokenType.Indent,
                DefaultTokenType.Identifier,
                DefaultTokenType.Space,
                DefaultTokenType.Indent,
                DefaultTokenType.Identifier,
                DefaultTokenType.Space,
                DefaultTokenType.Dedent,
                DefaultTokenType.Identifier,
                DefaultTokenType.Space
            });
            yield return new TestFixtureData("Numbers", new[]
                {
                    "abcd123",
                    ".567",
                    "  ",
                    "3412",
                    "abc",
                    " ",
                    "4.23",
                    ".",
                    " ",
                    "-",
                    ".3"
                },
                new[]
                {
                    DefaultTokenType.Identifier,
                    DefaultTokenType.Real,
                    DefaultTokenType.Space,
                    DefaultTokenType.Integer,
                    DefaultTokenType.Identifier,
                    DefaultTokenType.Space,
                    DefaultTokenType.Real,
                    DefaultTokenType.Dot,
                    DefaultTokenType.Space,
                    DefaultTokenType.Minus,
                    DefaultTokenType.Real
                });
        }
    }
}

[TestFixtureSource(typeof(LexerTestsFixtureData), nameof(LexerTestsFixtureData.FixtureParams))]
public class LexerTests
{
    [SetUp]
    public void Setup()
    {
        Console.WriteLine($"Test {_name}\nContents:\n{_contents}\n------------------------------");
    }

    private readonly QLexer _lexer;
    private readonly List<Token> _tokens = new();
    private readonly string _contents;
    private readonly DefaultTokenType[] _expectedTokenTypes;
    private readonly string[] _expectedTokenContents;
    private readonly string _name;
    private readonly FileInformation _fileInformation;


    public LexerTests(string name, string[] expectedContents, DefaultTokenType[] expectedTokenTypes)
    {
        _expectedTokenContents = expectedContents;
        _expectedTokenTypes = expectedTokenTypes;
        _name = name;
        _contents = string.Join("", _expectedTokenContents);
        _fileInformation = new FileInformation("TestFile");
        var stream = new SourceInputStream(_fileInformation,
            new MemoryStream(Encoding.UTF8.GetBytes(_contents)));
        _lexer = new QLexer(stream, _fileInformation);
    }

    [Test]
    public void Test()
    {
        var token = _lexer.NextToken();
        Console.WriteLine(token);
        while (token.TokenType != TokenConstants.Eof)
        {
            _tokens.Add(token);
            token = _lexer.NextToken();
            Console.WriteLine(token);
        }

        Assert.That(_expectedTokenTypes, Has.Length.EqualTo(_tokens.Count));
        Console.WriteLine("Tokens: ");
        for (var index = 0; index < _tokens.Count; index++)
        {
            var tkn = _tokens[index];
            Console.WriteLine(tkn);
            Assert.Multiple(() =>
            {
                Assert.That((int)_expectedTokenTypes[index], Is.EqualTo(tkn.TokenType));
                Assert.That(_expectedTokenContents[index], Is.EqualTo(tkn.Content));
            });
        }

        Assert.Pass();
    }
}