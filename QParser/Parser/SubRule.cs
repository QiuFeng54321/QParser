using System.Text;
using QParser.Lexer;

namespace QParser.Parser;

public abstract class SubRule
{
    public string Name = "?";
    public Grammar Grammar;

    protected SubRule(Grammar grammar)
    {
        Grammar = grammar;
    }

    public static SubRuleWithAction operator *(SubRule subRule, Action<SubRule> action)
    {
        return new SubRuleWithAction(subRule, action);
    }

    public override string ToString() => Name;
}