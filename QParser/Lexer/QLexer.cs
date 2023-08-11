using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using QParser.Lexer.States;

namespace QParser.Lexer;

public class QLexer : IEnumerable<Token>
{
    public static readonly Token Eof = new(TokenConstants.Eof, "$");
    private readonly Stack<int> _indentStack = new();

    private readonly Dictionary<string, DefaultTokenType> _plainSymbols = new()
    {
        ["DECLARE"] = DefaultTokenType.Declare,
        ["AS"] = DefaultTokenType.As,
        ["IF"] = DefaultTokenType.If,
        ["THEN"] = DefaultTokenType.Then,
        ["ELSE"] = DefaultTokenType.Else,
        ["FOR"] = DefaultTokenType.For,
        ["TO"] = DefaultTokenType.To,
        [">"] = DefaultTokenType.GreaterThan,
        ["<"] = DefaultTokenType.LessThan,
        [">="] = DefaultTokenType.GreaterThanOrEqualTo,
        ["<="] = DefaultTokenType.LessThanOrEqualTo,
        ["="] = DefaultTokenType.Equal,
        ["<>"] = DefaultTokenType.NotEqual,
        ["+"] = DefaultTokenType.Plus,
        ["-"] = DefaultTokenType.Minus,
        ["*"] = DefaultTokenType.Multiply,
        ["/"] = DefaultTokenType.Divide,
        ["DIV"] = DefaultTokenType.IntDivide,
        ["^"] = DefaultTokenType.Power,
        ["&"] = DefaultTokenType.BitAnd,
        ["|"] = DefaultTokenType.BitOr,
        ["("] = DefaultTokenType.OpenParen,
        [")"] = DefaultTokenType.CloseParen,
        ["["] = DefaultTokenType.OpenBracket,
        ["]"] = DefaultTokenType.CloseBracket,
        ["{"] = DefaultTokenType.OpenBrace,
        ["}"] = DefaultTokenType.CloseBrace,
        ["<-"] = DefaultTokenType.Assignment,
        ["."] = DefaultTokenType.Dot,
        [":"] = DefaultTokenType.Colon,
        [";"] = DefaultTokenType.Semicolon,
        [","] = DefaultTokenType.Comma,
        ["?"] = DefaultTokenType.QuestionMark
    };

    private readonly RootState _rootState;
    private readonly SourceInputStream _stream;
    private readonly StringBuilder _stringBuilder = new();
    private CharPosition _charPosition;
    private CharPosition _currentCharPosition;
    private State _currentState;
    private char _lookaheadChar;
    private bool _reserveOneChar;

    public QLexer(SourceInputStream sourceInputStream, FileInformation fileInformation)
    {
        _stream = sourceInputStream;
        _charPosition = new CharPosition(0, 0, fileInformation);
        _currentState = _rootState = new RootState(null);
        _indentStack.Push(0);
        RegisterPlainSymbols();
        RegisterNumberStateTransition();
        RegisterComment();
    }

    private bool NextCharIsEof => _lookaheadChar == '\0' && _stream.EndOfStream;

    public IEnumerator<Token> GetEnumerator()
    {
        Token token;
        do
        {
            token = NextToken();
            yield return token;
        } while (token.TokenType != TokenConstants.Eof);
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    private void RegisterPlainSymbols()
    {
        foreach (var (str, tokenType) in _plainSymbols) _rootState.RootTrieState.Add(str, (int)tokenType);
    }

    private void RegisterNumberStateTransition()
    {
        foreach (var prefix in new[] { "", "." })
        {
            for (var i = 0; i < 10; i++)
                _rootState.RootTrieState.Add(prefix + i,
                    prefix is not "." ? new IntegerState(null) : new RealState(null));

            if (prefix is not ("" or ".")) _rootState.RootTrieState.Add(prefix + ".", new IntegerState(null));
        }
    }

    private void RegisterComment()
    {
        _rootState.RootTrieState.Add("//", new SingleLineCommentState(null));
    }

    private void ReserveOneChar()
    {
        // Console.WriteLine($"Reserve '{Regex.Escape(_lookaheadChar.ToString())}'");
        _reserveOneChar = true;
    }

    private char NextChar()
    {
        if (_reserveOneChar)
        {
            _reserveOneChar = false;
            // Console.WriteLine($"Release {Regex.Escape(_lookaheadChar.ToString())}");
            return _lookaheadChar;
        }

        if (_stream.EndOfStream)
        {
            if (_lookaheadChar != '\0') _stream.FinishLineRecording();
            return _lookaheadChar = '\0';
        }

        _lookaheadChar = (char)_stream.Read();
        _currentCharPosition = _charPosition;
        _charPosition.Feed(_lookaheadChar);
        return _lookaheadChar;
    }

    /// <summary>
    ///     Reads the next token. If the stream is at its end, returns <see cref="TokenConstants.Eof" />
    /// </summary>
    /// <returns> The next token </returns>
    /// <exception cref="FormatException">The token processor cannot identify this token</exception>
    public Token NextToken()
    {
        if (NextCharIsEof) return Eof;
        _currentState = _rootState;
        _stringBuilder.Clear();
        var accept = false;
        var ignore = false;
        var startPos = _charPosition;
        var firstTime = true;
        var lastPos = _charPosition;
        while (!accept)
        {
            var c = NextChar();
            if (firstTime)
            {
                startPos = _currentCharPosition;
                firstTime = false;
            }

            if (SpaceState.CheckAll(c))
            {
                if (_currentState.OfferTerminate())
                {
                    // Accept and break
                    ReserveOneChar();
                    break;
                }
            }
            else if (startPos.Column == 0 && _stringBuilder.Length == 0 && _indentStack.Peek() != 0)
            {
                // This is to look for dedent to 0 space.
                // happens when at start of line with no space ahead, and the current indent level is not 0
                ReserveOneChar();
                _currentState = new SpaceState(null) { CanBeIndent = true };
                break;
            }

            var transition = _currentState.NextState(c);
            // Console.WriteLine($"'{Regex.Escape(c.ToString())}': {transition}");
            var consume = transition.Flag.HasFlag(StateTransitionFlag.ConsumeChar);

            if (transition.Flag.HasFlag(StateTransitionFlag.Accept))
            {
                accept = true;
                ignore = transition.Flag.HasFlag(StateTransitionFlag.Ignore);
            }

            if (transition.State != null) _currentState = transition.State;
            if (transition.Flag.HasFlag(StateTransitionFlag.Error))
                throw new FormatException(
                    $"Unrecognized token: \"{_stringBuilder}\" at {_currentCharPosition.ToStringWithFile()}," +
                    $"Current state {_currentState}, {transition}");

            if (consume)
            {
                if (c != '\0') _stringBuilder.Append(c);
            }
            else
            {
                ReserveOneChar();
            }

            lastPos = _currentCharPosition;
        }

        var str = _stringBuilder.ToString();
        if (_currentState is RootState && NextCharIsEof) return Eof;
        var token = _currentState.Accept(str);
        token.SourceRange.Start = startPos;
        token.SourceRange.End = lastPos;
        token.Ignore = ignore;
        // Transform into indent or dedent for space tokens
        if (token.SourceRange.Start.Column != 0 || _currentState is not SpaceState { CanBeIndent: true }) return token;
        if (token.Content.Length > _indentStack.Peek())
        {
            token.TokenType = (int)DefaultTokenType.Indent;
            token.Ignore = false;
            _indentStack.Push(token.Content.Length);
        }
        else if (token.Content.Length < _indentStack.Peek())
        {
            token.TokenType = (int)DefaultTokenType.Dedent;
            token.Ignore = false;
            _indentStack.Pop();
        }

        return token;
    }

    public void Close()
    {
        _stream.Close();
    }
}