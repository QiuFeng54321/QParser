namespace QParser.Lexer.States;

public class StringState : State
{
    private int _consecutiveSlashCount;

    public StringState(State? parentState) : base(parentState)
    {
    }

    public override StateTransition NextState(char c)
    {
        if (c == '"' && _consecutiveSlashCount % 2 == 0) return StateTransition.AcceptConsume;
        if (c == '\\')
            _consecutiveSlashCount++;
        else
            _consecutiveSlashCount = 0;

        return new StateTransition(this, StateTransitionFlag.ConsumeChar);
    }

    public override Token Accept(string str)
    {
        return new Token(DefaultTokenType.String.ToInt(), str);
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