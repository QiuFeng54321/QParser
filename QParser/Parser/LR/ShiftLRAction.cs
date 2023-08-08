namespace QParser.Parser.LR;

public class ShiftLRAction : LRAction
{
    public int ShiftState;

    public ShiftLRAction(int shiftState)
    {
        ShiftState = shiftState;
    }

    public override string ToString()
    {
        return $"s{ShiftState}";
    }
}