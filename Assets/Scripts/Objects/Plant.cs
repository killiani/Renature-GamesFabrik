using UnityEngine;

public class Plant
{
    public enum PlantType { Farn, Mangrove, Croton, Alocasia, Teaktree }
    public PlantType Type { get; private set; }
    public float GrowthTime { get; private set; }

    public Plant(PlantType type, float growthTime)
    {
        Type = type;
        GrowthTime = growthTime;
    }

    public void Grow()
    {
        // Logik für das Wachstum der Pflanze
        Debug.Log($"Growing {Type} plant with growth time of {GrowthTime} seconds.");
    }
}
