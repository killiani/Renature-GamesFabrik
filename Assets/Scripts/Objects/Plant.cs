using UnityEngine;

public class Plant
{
    public enum PlantType { Farn, Mangrove, Croton, Alocasia, Teaktree }
    public PlantType Type { get; private set; }
    public float GrowthTime { get; private set; }
    public int RequiredBlocks { get; private set; }

    public Plant(PlantType type, float growthTime)
    {
        Type = type;
        GrowthTime = growthTime;
        RequiredBlocks = GetRequiredBlocks(type);
    }

    public static int GetRequiredBlocks(PlantType type)
    {
        switch (type)
        {
            case PlantType.Farn:
                return 1;
            case PlantType.Mangrove:
                return 4;
            case PlantType.Croton:
                return 1;
            case PlantType.Alocasia:
                return 1;
            case PlantType.Teaktree:
                return 2;
            default:
                return 1; // Standardwert, falls ein unbekannter Pflanzentyp übergeben wird
        }
    }
}

