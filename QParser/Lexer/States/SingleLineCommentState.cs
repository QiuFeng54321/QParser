namespace QParser.Lexer.States;

public class SingleLineCommentState : State
{
    public SingleLineCommentState(State? parentState) : base(parentState)
    {
    }

    public override StateTransition NextState(char c)
    {
        if (c != '\n' && c != '\0') return new StateTransition(this, StateTransitionFlag.ConsumeChar);
        return new StateTransition(null, StateTransitionFlag.Accept | StateTransitionFlag.Ignore);
    }

    public override Token Accept(string str)
    {
        return new Token(DefaultTokenType.Comment.ToInt(), str);
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