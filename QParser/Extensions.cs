using System.Collections.Generic;

namespace QParser;

public static class Extensions
{
    /// <summary>
    ///     Joins two <see cref="HashSet{T}" />s and checks if the first hashset is changed (in length)
    /// </summary>
    /// <param name="hashSet">The set to merge the other set into</param>
    /// <param name="hashSet2">The set to be merged in <paramref name="hashSet" /></param>
    /// <typeparam name="T">The type of the elements of <paramref name="hashSet" /></typeparam>
    /// <returns>If <paramref name="hashSet" /> has changed its length</returns>
    public static bool UnionAndCheckChange<T>(this HashSet<T> hashSet, HashSet<T> hashSet2)
    {
        if (ReferenceEquals(hashSet, hashSet2)) return false;
        var lengthBefore = hashSet.Count;
        hashSet.UnionWith(hashSet2);
        return lengthBefore != hashSet.Count;
    }

    public static bool TryAdd<TKey, TValue>(this Dictionary<TKey, TValue> dict, TKey key, TValue value)
    {
        if (dict.ContainsKey(key)) return false;
        dict.Add(key, value);
        return true;
    }

    public static void Deconstruct<TKey, TValue>(this KeyValuePair<TKey, TValue> pair, out TKey key, out TValue value)
    {
        key = pair.Key;
        value = pair.Value;
    }
}