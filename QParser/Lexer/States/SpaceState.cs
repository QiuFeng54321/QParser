using QParser.Lexer.Tokens;

namespace QParser.Lexer.States;

public class SpaceState : State
{
    public static readonly SpaceState Identity = new (null);
    public static readonly StateTransition Transition = new(Identity, StateTransitionFlag.ConsumeChar);
    
    public SpaceState(State? parentState) : base(parentState)
    {
    }

    public override StateTransition NextState(char c)
    {
        // '      '
        if (char.IsWhiteSpace(c))
        {
            return new(this, StateTransitionFlag.ConsumeChar);
        }
        // '      \n'
        if (c == '\n')
        {
            return new(this, StateTransitionFlag.Accept | StateTransitionFlag.ConsumeChar | StateTransitionFlag.Ignore);
        }
        // '   d', don't consume d, accept and mark ignore
        return new(this, StateTransitionFlag.Accept | StateTransitionFlag.Ignore);
    }

    public override Token Accept(string str)
    {
        return new Token(TokenType.Space, str);
    }

    public override StateTransition MakeTransitionToThis()
    {
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