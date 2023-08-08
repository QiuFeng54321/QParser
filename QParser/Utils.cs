using System.Runtime.CompilerServices;

namespace QParser;

internal static class Utils
{
    public static int ToInt<TEnum>(this TEnum value) where TEnum : Enum
    {
        return Unsafe.As<TEnum, int>(ref value);
    }
}