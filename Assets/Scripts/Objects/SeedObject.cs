using UnityEngine;

public class SeedObject : MonoBehaviour
{
    public Seed.SeedType seedType;
    public float growthTime;

    private void Start()
    {
        // Wenn die Wachstumszeit nicht gesetzt ist, eine zufällige Wachstumszeit festlegen
        if (growthTime <= 0)
        {
            growthTime = Random.Range(50f, 80f); // Beispielwerte für Wachstumszeit
        }
    }

    public Seed GetSeed()
    {
        return new Seed(seedType, growthTime, 0);
    }
}
