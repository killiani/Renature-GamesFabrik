using System.Collections.Generic;
using UnityEngine;

public class Backpack : MonoBehaviour
{
    private List<GameObject> seeds = new List<GameObject>();

    // Methode zum Hinzufügen eines Samens
    public void AddSeed(GameObject seed)
    {
        seeds.Add(seed);
        seed.SetActive(false); // Samen deaktivieren, nachdem er aufgesammelt wurde
        Debug.Log("Seed added to backpack. Total seeds: " + seeds.Count);
    }

    // Methode, um die Anzahl der Samen zu erhalten
    public int GetSeedCount()
    {
        return seeds.Count;
    }
}
