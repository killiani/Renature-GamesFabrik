using System.Collections.Generic;
using UnityEngine;

public class Backpack : MonoBehaviour
{
    private List<Seed> seeds = new List<Seed>();

    // Methode zum Hinzufügen eines Samens
    public void AddSeed(Seed seed)
    {
        seeds.Add(seed);
        //seed.SetActive(false); // Samen deaktivieren, nachdem er aufgesammelt wurde
        Debug.Log($"Seed added to backpack: {seed.Type} with a grow Time of {seed.GrowthTime}. Total seeds: {seeds.Count}");
    }

    public int GetSeedCount()
    {
        return seeds.Count;
    }

    public List<Seed> GetAllSeeds()
    {
        return seeds;
    }

    // Samen wählen und aus rucksack entfernen
    public Seed GetAndRemoveLastSeed()
    {
        if (seeds.Count > 0)
        {
            Seed lastSeed = seeds[seeds.Count - 1];
            seeds.RemoveAt(seeds.Count - 1);
            return lastSeed;
        }
        return null;
    }
}
