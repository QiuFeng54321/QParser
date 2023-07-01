using System.Text;
using QParser.Lexer.States;
using QParser.Lexer.Tokens;

namespace QParser.Lexer;

public class QLexer
{
    private readonly StreamReader _stream;
    private char _lookaheadChar;
    private State _currentState;
    private readonly RootState _rootState;
    private CharPosition _charPosition;
    private readonly StringBuilder _stringBuilder = new();
    private bool _reserveOneChar;
    private readonly Stack<int> _indentStack = new();

    private readonly Dictionary<string, TokenType> _plainSymbols = new()
    {
        ["DECLARE"] = TokenType.Declare,
        ["AS"] = TokenType.As,
        ["IF"] = TokenType.If,
        ["THEN"] = TokenType.Then,
        ["ELSE"] = TokenType.Else,
        ["FOR"] = TokenType.For,
        ["TO"] = TokenType.To,
        [">"] = TokenType.GreaterThan,
        ["<"] = TokenType.LessThan,
        [">="] = TokenType.GreaterThanOrEqualTo,
        ["<="] = TokenType.LessThanOrEqualTo,
        ["="] = TokenType.Equal,
        ["<>"] = TokenType.NotEqual,
        ["("] = TokenType.OpenParen,
        [")"] = TokenType.CloseParen,
        ["["] = TokenType.OpenBracket,
        ["]"] = TokenType.CloseBracket,
        ["{"] = TokenType.OpenBrace,
        ["}"] = TokenType.CloseBrace,
        ["<-"] = TokenType.Assignment,
    };

    public QLexer(Stream stream, string filePath)
    {
        _stream = new StreamReader(stream, Encoding.UTF8);
        _charPosition = new CharPosition(0, 0, filePath);
        _currentState = _rootState = new RootState(null);
        RegisterPlainSymbols();
    }

    private void RegisterPlainSymbols()
    {
        foreach (var (str, tokenType) in _plainSymbols)
        {
            _rootState.RootTrieState.Add(str, tokenType);
        }
    }

    private void ReserveOneChar()
    {
        Console.WriteLine($"Reserve {_lookaheadChar}");
        _reserveOneChar = true;
    }

    private char NextChar()
    {
        if (_reserveOneChar)
        {
            _reserveOneChar = false;
            Console.WriteLine($"Release {_lookaheadChar}");
            return _lookaheadChar;
        }

        if (_stream.EndOfStream) return '\0';
        _lookaheadChar = (char)_stream.Read();
        _charPosition.Feed(_lookaheadChar);
        return _lookaheadChar;
    }

    /// <summary>
    /// Reads the next token. If the stream is at its end, returns <see cref="TokenType.Eof"/>
    /// </summary>
    /// <returns> The next token </returns>
    /// <exception cref="FormatException">The token processor cannot identify this token</exception>
    public Token NextToken()
    {
        if (_stream.EndOfStream) return new Token(TokenType.Eof, "$");
        _currentState = _rootState;
        _stringBuilder.Clear();
        var accept = false;
        var ignore = false;
        var startPos = _charPosition;
        while (!accept)
        {
            var curPos = _charPosition;
            var c = NextChar();
            if (SpaceState.CheckAll(c))
            {
                if (_currentState.OfferTerminate())
                {
                    // Accept and break
                    ReserveOneChar();
                    break;
                }
            }
            var transition = _currentState.NextState(c);
            Console.WriteLine($"'{c}': {transition}");
            var consume = transition.Flag.HasFlag(StateTransitionFlag.ConsumeChar);
            
            if (transition.Flag.HasFlag(StateTransitionFlag.Accept))
            {
                accept = true;
                ignore = transition.Flag.HasFlag(StateTransitionFlag.Ignore);
            }

            if (transition.State != null) _currentState = transition.State;
            if (transition.Flag.HasFlag(StateTransitionFlag.Error))
            {
                throw new FormatException($"Unrecognized token: \"{_stringBuilder}\" at {_charPosition.ToStringWithFile()}");
            }

            if (consume)
            {
                if (c != '\0') _stringBuilder.Append(c);
            }
            else
            {
                ReserveOneChar();
            }
        }

        var str = _stringBuilder.ToString();
        var token = _currentState.Accept(str);
        token.Start = startPos;
        token.End = _charPosition;
        token.Ignore = ignore;
        return token;
    }
}