namespace Content.Shared._ES.Utility;

public interface IMergeable<in TOther>
{
    /// <summary>
    ///     Merges two values in a dictionary, returns false if they cancelled out and need removed.
    /// </summary>
    public bool Merge(TOther other);
}

public static class MergeExtensions
{
    public static void MergeKey<TKey, TValue>(this IDictionary<TKey, TValue> dict, TKey key, TValue value)
        where TValue: IMergeable<TValue>
    {
        if (dict.TryGetValue(key, out var other))
        {
            var keep = other.Merge(value);
            if (!keep)
                dict.Remove(key);
        }
        else
        {
            dict.Add(key, value);
        }
    }
}
