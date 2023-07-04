using System.Text;

namespace QParser.Parser;

public class SubRuleWithAction : SubRule
{
    public readonly SubRule InternalSubRule;
    public readonly Action<SubRule>[] Actions;

    public SubRuleWithAction(SubRule internalSubRule, IEnumerable<Action<SubRule>> actions) : this(internalSubRule,
        actions.ToArray()){}
    public SubRuleWithAction(SubRule internalSubRule, params Action<SubRule>[] actions) : base(internalSubRule.Grammar)
    {
        InternalSubRule = internalSubRule;
        Actions = actions;
        Name = InternalSubRule.Name;
    }
    public static SubRuleWithAction operator *(SubRuleWithAction subRule, Action<SubRule> action)
    {
        return new SubRuleWithAction(subRule, subRule.Actions.Append(action));
    }

    public override string ToString() => $"{InternalSubRule} [#{Actions.Length}]";

}