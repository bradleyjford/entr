using System.Collections;

namespace Entr.Domain;

// Adapted from Josh Bloch's book "Effective Java" - http://www.amazon.com/dp/0321356683/.
public static class HashCodeUtility
{
    const int OddPrimeNumber = 37;

    public static readonly int Seed = 23;

    static int GetFirstTerm(int seed)
    {
        return OddPrimeNumber * seed;
    }

    public static int Hash(int seed, bool value)
    {
        return GetFirstTerm(seed) + (value ? 1 : 0);
    }

    public static int Hash(int seed, char value)
    {
        return GetFirstTerm(seed) + value;
    }

    public static int Hash(int seed, int value)
    {
        return GetFirstTerm(seed) + value;
    }

    public static int Hash(int seed, long value)
    {
        return GetFirstTerm(seed) + (int)(value ^ (value >> 32));
    }

    public static int Hash(int seed, float value)
    {
        return Hash(seed, (int)BitConverter.DoubleToInt64Bits(value));
    }

    public static int Hash(int seed, double value)
    {
        return Hash(seed, BitConverter.DoubleToInt64Bits(value));
    }

    public static int Hash(int seed, Guid value)
    {
        return Hash(seed, value.GetHashCode());
    }
    
    public static int Hash(int seed, Type value)
    {
        return Hash(seed, value.GetHashCode());
    }
    
    public static int Hash(int seed, object obj)
    {
        var result = seed;

        if (obj is null)
        {
            return Hash(result, 0);
        }
        
        if (obj is IEnumerable items)
        {
            foreach (var item in items)
            {
                result = Hash(result, item);
            }

            return result;
        }

        return Hash(result, obj.GetHashCode());
    }
}
