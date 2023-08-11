namespace QParser.Lexer.States;

public class SingleLineCommentState : State
{
    public SingleLineCommentState(State? parentState) : base(parentState)
    {
    }

    public override StateTransition NextState(char c)
    {
        if (c != '\n') return new StateTransition(this, StateTransitionFlag.ConsumeChar);
        return StateTransition.Accept;
    }

    public override Token Accept(string str)
    {
        return new Token(DefaultTokenType.Comment.ToInt(), str) { Ignore = true };
    }

    public override StateTransition MakeTransitionToThis()
    {
        return new StateTransition(this, StateTransitionFlag.ConsumeChar);
    }

    public override bool OfferTerminate()
    {
        return false;
    }
}