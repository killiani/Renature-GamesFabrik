using UnityEngine;

public class Plant
{
    public enum PlantType { Farn, Mangrove, Croton, Alocasia, Teaktree }
    public PlantType Type { get; private set; }
    public float GrowthTime { get; private set; }
    public int RequiredBlocks { get; private set; }
    public int GrowthStatus { get; private set; }

    public Plant(PlantType type, float growthTime, int growthStatus)
    {
        Type = type;
        GrowthTime = growthTime;
        RequiredBlocks = GetRequiredBlocks(type);
        GrowthStatus = growthStatus;
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

    public static int GetRequiredBlocksByCardName(string seedCardName)
    {

        // Entferne das Suffix "Card" aus dem Namen, um den eigentlichen Seed-Namen zu erhalten
        string seedName = seedCardName.Replace("Card", "");

        switch (seedName)
        {
            case "Farn":
                return 1;
            case "Mangrove":
                return 4;
            case "Croton":
                return 1;
            case "Alocasia":
                return 1;
            case "Teaktree":
                return 2;
            default:
                return 1; // Standardwert, falls ein unbekannter Pflanzentyp übergeben wird

        }
    }
}

