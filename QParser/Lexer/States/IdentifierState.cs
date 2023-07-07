using QParser.Lexer.Tokens;

namespace QParser.Lexer.States;

public class IdentifierState : State
{
    public static readonly IdentifierState Identity = new(null);

    public IdentifierState(State? parentState) : base(parentState)
    {
    }

    public override StateTransition NextState(char c)
    {
        if (CheckBody(c)) return new StateTransition(this, StateTransitionFlag.ConsumeChar);
        return StateTransition.Accept;
    }

    public static bool CheckBody(char c)
    {
        return char.IsLetterOrDigit(c) || c is '_' or '$';
    }

    public static bool CheckBeginning(char c)
    {
        return char.IsLetter(c) || c is '_' or '$';
    }

    public override Token Accept(string str)
    {
        return new IdentifierToken(str);
    }

    public override StateTransition MakeTransitionToThis()
    {
        return new StateTransition(this, StateTransitionFlag.ConsumeChar);
    }

    public override bool OfferTerminate()
    {
        return true;
    }
}