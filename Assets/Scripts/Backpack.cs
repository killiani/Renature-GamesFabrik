using System.Collections.Generic;
using UnityEngine;
using static Seed;

public class Backpack : MonoBehaviour
{
    private List<Seed> seeds = new List<Seed>();
    private BackpackController backpackController;

    [SerializeField] private List<GameObject> seedObjects; // Liste aller Samen-Objekte im Rucksack

    void Start()
    {
        backpackController = FindObjectOfType<BackpackController>();
        if (backpackController == null)
        {
            Debug.LogError("BackpackController not found!");
        }

        UpdateSeedDisplay();
    }

    public void AddSeed(Seed seed)
    {
        seeds.Add(seed);
        //Debug.Log($"Seed added to backpack: {seed.Type} with a grow Time of {seed.GrowthTime}. Total seeds: {seeds.Count}");

        if (backpackController != null)
        {
            backpackController.RefreshSeedCards();
        }
        else
        {
            Debug.LogError("BackpackController reference is null");
        }

        UpdateSeedDisplay();
    }

    public int GetSeedCount()
    {
        return seeds.Count;
    }

    public int GetSeedCountByType(SeedType type)
    {
        int count = 0;
        foreach (Seed seed in seeds)
        {
            if (seed.Type == type)
            {
                count++;
            }
        }
        return count;
    }

    public List<Seed> GetAllSeeds()
    {
        return seeds;
    }

    /* 
     * Hack: 
     Da das suchen mit dem index zu unsicher war, wurda auf den Namen ausgewichen. 
     Dem Namen wird dann hier das "Card" entzogen und geschaut welcher Typ dazu passt.
     */
    public Seed GetAndRemoveSeedByName(int index, string seedCardName)
    {
        // Entferne das Suffix "Card" aus dem Namen, um den eigentlichen Seed-Namen zu erhalten
        string seedName = seedCardName.Replace("Card", "");

        // Versuche, den Seed-Namen in den entsprechenden SeedType-Enum-Wert zu konvertieren
        if (System.Enum.TryParse(seedName, out Seed.SeedType seedType))
        {
            // Suche nach dem Seed mit dem angegebenen Typ
            Seed seedToRemove = seeds.Find(seed => seed.Type == seedType);

            if (seedToRemove != null)
            {
                // Entferne den Seed aus der Liste
                seeds.Remove(seedToRemove);

                // Aktualisiere die Anzeige der Samen
                UpdateSeedDisplay();

                return seedToRemove;
            }
        }
        return null;
    }



    // Methode zum Aktualisieren der Samen-Anzeige basierend auf der Anzahl der Samen im Rucksack
    private void UpdateSeedDisplay()
    {
        for (int i = 0; i < seedObjects.Count; i++)
        {
            seedObjects[i].SetActive(i < seeds.Count);
        }
    }
}
