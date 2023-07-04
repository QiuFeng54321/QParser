using QParser.Lexer;
using QParser.Parser;

namespace QParser;

public static class Extensions
{
    
    public static SubRuleWithAction A(this SubRule subRule, Action<SubRule> action)
    {
        return new SubRuleWithAction(subRule, action);
    }
}