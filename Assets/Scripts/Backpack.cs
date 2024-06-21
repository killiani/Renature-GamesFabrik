using System.Collections.Generic;
using UnityEngine;
using static Seed;

public class Backpack : MonoBehaviour
{
    private List<Seed> seeds = new List<Seed>();
    private BackpackController backpackController;

    void Start()
    {
        backpackController = FindObjectOfType<BackpackController>();
        if (backpackController == null)
        {
            Debug.LogError("BackpackController not found!");
        }
    }

    public void AddSeed(Seed seed)
    {
        seeds.Add(seed);
        Debug.Log($"Seed added to backpack: {seed.Type} with a grow Time of {seed.GrowthTime}. Total seeds: {seeds.Count}");

        if (backpackController != null)
        {
            backpackController.RefreshSeedCards();
        }
        else
        {
            Debug.LogError("BackpackController reference is null");
        }
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

    // Samen w�hlen und aus Rucksack entfernen
    public Seed GetAndRemoveSeedAt(int index)
    {
        if (index >= 0 && index < seeds.Count)
        {
            Seed seed = seeds[index];
            seeds.RemoveAt(index);
            return seed;
        }
        return null;
    }
}
