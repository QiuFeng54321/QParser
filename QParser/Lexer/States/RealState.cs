using QParser.Lexer.Tokens;

namespace QParser.Lexer.States;

public class RealState : State
{
    public RealState(State? parentState) : base(parentState)
    {
    }

    public override StateTransition NextState(char c)
    {
        if (Check(c)) return new StateTransition(this, StateTransitionFlag.ConsumeChar);
        return StateTransition.Accept;
    }

    public static bool Check(char c)
    {
        return c is >= '0' and <= '9';
    }

    public override Token Accept(string str)
    {
        return new RealToken(str);
    }

    /// <summary>
    ///     Only when next char is . will this be called
    /// </summary>
    /// <returns></returns>
    public override StateTransition MakeTransitionToThis()
    {
        return new StateTransition(this, StateTransitionFlag.ConsumeChar);
    }

    public override bool OfferTerminate()
    {
        return true;
    }
}