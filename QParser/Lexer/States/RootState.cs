using QParser.Lexer.Tokens;

namespace QParser.Lexer.States;

public class RootState : State
{
    public readonly TrieState RootTrieState;

    public RootState(State? parentState) : base(parentState)
    {
        RootTrieState = new TrieState(this, '\0', true);
    }

    public override StateTransition NextState(char c)
    {
        if (c == '\0') return StateTransition.Accept;
        if (SpaceState.CheckAll(c)) return SpaceState.Identity.MakeTransitionToThis();
        // if (IntegerState.Check(c)) return new IntegerState(this).MakeTransitionToThis();
        return RootTrieState.MakeTransitionToThis();
    }

    public override Token Accept(string str)
    {
        throw new NotImplementedException();
    }

    public override StateTransition MakeTransitionToThis()
    {
        return new StateTransition(this, StateTransitionFlag.Accept);
    }

    public override bool OfferTerminate()
    {
        return false;
    }
}