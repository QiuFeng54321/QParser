using QParser.Lexer.Tokens;

namespace QParser.Lexer.States;

public class RootState : State
{
    public readonly TrieState RootTrieState;
    public override StateTransition NextState(char c)
    {
        if (c == '\0') return StateTransition.Accept;
        if (SpaceState.CheckAll(c)) return SpaceState.Identity.MakeTransitionToThis();
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

    public RootState(State? parentState) : base(parentState)
    {
        RootTrieState = new TrieState(this, '\0', true);
    }
}