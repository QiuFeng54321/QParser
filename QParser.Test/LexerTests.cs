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
                TokenType.Declare,
                TokenType.Space,
                TokenType.Identifier,
                TokenType.Space,
                TokenType.As,
                TokenType.GreaterThanOrEqualTo,
                TokenType.Equal,
                TokenType.NotEqual,
                TokenType.Identifier
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
                TokenType.Indent,
                TokenType.Identifier,
                TokenType.Space,
                TokenType.Indent,
                TokenType.Identifier,
                TokenType.Space,
                TokenType.Dedent,
                TokenType.Identifier,
                TokenType.Space
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
                    "-.3"
                },
                new[]
                {
                    TokenType.Identifier,
                    TokenType.Real,
                    TokenType.Space,
                    TokenType.Integer,
                    TokenType.Identifier,
                    TokenType.Space,
                    TokenType.Real,
                    TokenType.Dot,
                    TokenType.Space,
                    TokenType.Real
                });
        }
    }
}

[TestFixtureSource(typeof(LexerTestsFixtureData), nameof(LexerTestsFixtureData.FixtureParams))]
public class LexerTests
{
    private readonly QLexer _lexer;
    private readonly List<Token> _tokens = new();
    private readonly string _contents;
    private readonly TokenType[] _expectedTokenTypes;
    private readonly string[] _expectedTokenContents;
    private readonly string _name;


    public LexerTests(string name, string[] expectedContents, TokenType[] expectedTokenTypes)
    {
        _expectedTokenContents = expectedContents;
        _expectedTokenTypes = expectedTokenTypes;
        _name = name;
        _contents = string.Join("", _expectedTokenContents);
        _lexer = new QLexer(new MemoryStream(Encoding.UTF8.GetBytes(_contents)), "TestFile");
    }

    [SetUp]
    public void Setup()
    {
        Console.WriteLine($"Test {_name}\nContents:\n{_contents}\n------------------------------");
    }

    [Test]
    public void Test()
    {
        var token = _lexer.NextToken();
        Console.WriteLine(token);
        while (token.TokenType != TokenType.Eof)
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
                Assert.That(_expectedTokenTypes[index], Is.EqualTo(tkn.TokenType));
                Assert.That(_expectedTokenContents[index], Is.EqualTo(tkn.Content));
            });
        }

        Assert.Pass();
    }
}