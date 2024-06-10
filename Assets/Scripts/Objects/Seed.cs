using UnityEngine;

public class Seed
{
    public enum SeedType { Farn, Mangrove, Croton, Alocasia, Teaktree }
    public SeedType Type { get; private set; }
    public float GrowthTime { get; private set; }

    public Seed(SeedType type, float growthTime)
    {
        Type = type;
        GrowthTime = growthTime;
    }

    public void Plant()
    {
        // Logik zum Pflanzen des Samens
        Debug.Log($"Planting {Type} with growth time of {GrowthTime} seconds.");
    }

    public static Seed GenerateRandomSeed()
    {
        System.Array seedTypes = System.Enum.GetValues(typeof(SeedType));
        SeedType randomType = (SeedType)seedTypes.GetValue(Random.Range(0, seedTypes.Length));
        float randomGrowthTime = Random.Range(5f, 8f); // Beispielwerte für Wachstumszeit
        return new Seed(randomType, randomGrowthTime);
    }
}
