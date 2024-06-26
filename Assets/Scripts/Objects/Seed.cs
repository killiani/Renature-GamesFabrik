using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;

public class Seed
{
    public enum SeedType { Farn, Mangrove, Croton, Alocasia, Teaktree }
    public SeedType Type { get; private set; }
    public float GrowthTime { get; private set; }

    public int GrowthStatus { get; private set; }

    public Seed(SeedType type, float growthTime, int growthStatus)
    {
        Type = type;
        GrowthTime = growthTime;
        GrowthStatus = growthStatus;
    }

    public static Seed GenerateRandomSeed()
    {
        System.Array seedTypes = System.Enum.GetValues(typeof(SeedType));
        SeedType randomType = (SeedType)seedTypes.GetValue(Random.Range(0, seedTypes.Length));
        float randomGrowthTime = Random.Range(5f, 8f); // Beispielwerte für Wachstumszeit
        return new Seed(randomType, randomGrowthTime, 0);
    }
}
