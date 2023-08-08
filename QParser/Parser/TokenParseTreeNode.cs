using System.Text;
using QParser.Lexer;
using QParser.Lexer.Tokens;

namespace QParser.Parser;

public class TokenParseTreeNode : ParseTreeNode
{
    public readonly Token Token;
    public readonly int TokenType;

    public TokenParseTreeNode(int tokenType, Token token)
    {
        TokenType = tokenType;
        Token = token;
    }

    public override SourceRange SourceRange => Token.SourceRange;

    public override void Format(StringBuilder stringBuilder, int indent)
    {
        stringBuilder.AppendLine(TokenType == TokenConstants.Epsilon ? "None" : Token.ToString());
    }
}