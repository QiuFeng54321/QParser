using QParser.Lexer.Tokens;

namespace QParser.Lexer.States;

public class IntegerState : State
{
    public IntegerState(State? parentState) : base(parentState)
    {
    }

    public override StateTransition NextState(char c)
    {
        if (Check(c)) return new StateTransition(this, StateTransitionFlag.ConsumeChar);
        if (c is '.') return new RealState(ParentState).MakeTransitionToThis();
        return StateTransition.Accept;
    }

    public static bool Check(char c)
    {
        return c is >= '0' and <= '9';
    }

    public override Token Accept(string str)
    {
        return new IntegerToken(TokenType.Integer, str);
    }

    public override StateTransition MakeTransitionToThis()
    {
        // None: We want to check the first digit
        return new StateTransition(this, StateTransitionFlag.None);
    }

    public override bool OfferTerminate()
    {
        return true;
    }
}