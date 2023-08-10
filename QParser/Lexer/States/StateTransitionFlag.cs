using System;

namespace QParser.Lexer.States;

[Flags]
public enum StateTransitionFlag
{
    None = 0,
    Accept = 1 << 0,
    Ignore = 1 << 1,
    Error = 1 << 2,
    ConsumeChar = 1 << 3
}