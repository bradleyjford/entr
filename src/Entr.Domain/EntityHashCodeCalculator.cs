using System;

namespace Entr.Domain;

static class EntityHashCodeCalculator
{
    static readonly Random RandomGenerator = new Random((int)ClockProvider.GetUtcNow().Ticks);

    public static int CalculateHashCode<TId>(Entity<TId> entity)
    {
        if (Equals(default(TId), entity.Id))
        {
            var random = RandomGenerator.Next(Int32.MinValue, Int32.MaxValue);

            var result = HashCodeUtility.Hash(HashCodeUtility.Seed, entity.GetType());
            return HashCodeUtility.Hash(result, random);
        }
        else
        {
            return entity.Id.GetHashCode();
        }
    }
}
