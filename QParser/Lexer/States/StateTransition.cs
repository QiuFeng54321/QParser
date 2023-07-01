namespace QParser.Lexer.States;

public class StateTransition
{
    public static readonly StateTransition AcceptConsume = new(null, StateTransitionFlag.Accept | StateTransitionFlag.ConsumeChar);
    public static readonly StateTransition Accept = new(null, StateTransitionFlag.Accept);
    public static readonly StateTransition Error = new(null, StateTransitionFlag.Error | StateTransitionFlag.ConsumeChar);
    public readonly State? State;
    public readonly StateTransitionFlag Flag;

    public StateTransition(State? state, StateTransitionFlag flag)
    {
        State = state;
        Flag = flag;
    }

    public override string ToString()
    {
        return $"Transition to {State}: {Flag:F}";
    }
}