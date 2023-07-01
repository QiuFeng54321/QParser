using QParser.Lexer.Tokens;

namespace QParser.Lexer.States;

/// <summary>
/// Indent state is the state to transition to after feeding a new line character.
/// 
/// </summary>
public class IndentState : State
{
    public static readonly IndentState Identity = new(null);
    public int IndentLevel;
    public IndentState(State? parentState) : base(parentState)
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
        return new StateTransition(this, StateTransitionFlag.ConsumeChar);
    }

    public override bool OfferTerminate()
    {
        throw new NotImplementedException();
    }
}