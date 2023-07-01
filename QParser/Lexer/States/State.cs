using QParser.Lexer.Tokens;

namespace QParser.Lexer.States;

public abstract class State
{
    public readonly State? ParentState;

    protected State(State? parentState)
    {
        ParentState = parentState;
    }

    /// <summary>
    /// Feeds the state with a character and transition into the next state.<br/>
    /// If the transition cannot be done, returns null.
    /// </summary>
    /// <param name="c">One character to feed</param>
    /// <returns>The next state. Null if cannot transition</returns>
    public abstract StateTransition NextState(char c);
    /// <summary>
    /// Accepts a string of characters and generates a token from it.
    /// </summary>
    /// <param name="str">The string to accept</param> 
    public abstract Token Accept(string str);
    /// <summary>
    /// Makes a transition to this state
    /// </summary>
    /// <returns>The transition</returns>
    public abstract StateTransition MakeTransitionToThis();

    /// <summary>
    /// When the lookahead char is a space / new line / eof, this method is called to test if this state
    /// wishes to terminate
    /// </summary>
    /// <returns>Whether the state wishes to terminate</returns>
    public abstract bool OfferTerminate();
}