using QParser.Lexer.Tokens;

namespace QParser.Lexer.States;

public class SpaceState : State
{
    public static readonly SpaceState Identity = new(null);
    private static readonly StateTransition Transition = new(Identity, StateTransitionFlag.None);
    public bool CanBeIndent = true;

    public SpaceState(State? parentState) : base(parentState)
    {
    }

    public override StateTransition NextState(char c)
    {
        // '      \n'
        if (c == '\n')
        {
            CanBeIndent = false;
            return new StateTransition(this,
                StateTransitionFlag.Accept | StateTransitionFlag.ConsumeChar | StateTransitionFlag.Ignore);
        }

        // '      '
        if (char.IsWhiteSpace(c)) return new StateTransition(this, StateTransitionFlag.ConsumeChar);
        // '   d', don't consume d, accept and mark ignore
        return new StateTransition(this, StateTransitionFlag.Accept | StateTransitionFlag.Ignore);
    }

    public override Token Accept(string str)
    {
        return new Token(TokenType.Space, str);
    }

    public override StateTransition MakeTransitionToThis()
    {
        CanBeIndent = true;
        return Transition;
    }

    public override bool OfferTerminate()
    {
        return false;
    }

    public static bool CheckAll(char c)
    {
        return char.IsWhiteSpace(c) || c is '\n' or '\0';
    }
}