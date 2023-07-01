using QParser.Lexer.Tokens;

namespace QParser.Lexer.States;

public class IntegerState : State
{
    public IntegerState(State? parentState) : base(parentState)
    {
    }

    public override StateTransition NextState(char c)
    {
        throw new NotImplementedException();
    }

    public override Token Accept(string str)
    {
        throw new NotImplementedException();
    }

    public override StateTransition MakeTransitionToThis()
    {
        throw new NotImplementedException();
    }

    public override bool OfferTerminate()
    {
        throw new NotImplementedException();
    }
}