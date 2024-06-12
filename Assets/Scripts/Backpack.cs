using System.Collections.Generic;
using UnityEngine;

public class Backpack : MonoBehaviour
{
    private List<Seed> seeds = new List<Seed>();

    public void AddSeed(Seed seed)
    {
        seeds.Add(seed);
    }

    public int GetSeedCount(Seed.SeedType seedType)
    {
        int count = 0;
        foreach (var seed in seeds)
        {
            if (seed.Type == seedType)
            {
                count++;
            }
        }
        return count;
    }

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

    public List<Seed> GetAllSeeds()
    {
        return new List<Seed>(seeds);
    }
}

