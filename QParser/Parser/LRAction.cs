using System.Text;

namespace QParser.Parser;

public class LRAction
{
    public CompositeNonterminal? ReduceProduction;
    public Rule? ReduceRule;
    public int ShiftState;
    public LRActionType Type;

    public LRAction()
    {
        Type = LRActionType.Accept;
    }

    public LRAction(int shiftState)
    {
        ShiftState = shiftState;
        Type = LRActionType.Shift;
    }

    public LRAction(Rule? reduceRule, CompositeNonterminal? reduceProduction)
    {
        Type = LRActionType.Reduce;
        ReduceRule = reduceRule;
        ReduceProduction = reduceProduction;
    }

    public override string ToString()
    {
        StringBuilder sb = new();
        sb.Append(Type switch
        {
            LRActionType.Shift => 's',
            LRActionType.Reduce => 'r',
            LRActionType.Accept => 'a',
            LRActionType.Error => '?',
            _ => throw new ArgumentOutOfRangeException()
        });
        if (Type is LRActionType.Shift)
            sb.Append(ShiftState);
        else if (Type is LRActionType.Reduce) sb.Append($"{{{ReduceRule} -> {ReduceProduction}}}");

        return sb.ToString();
    }
}