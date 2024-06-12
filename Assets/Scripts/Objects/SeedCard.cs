using UnityEngine;
using UnityEngine.UI;

public class SeedCard
{
    public Seed.SeedType SeedType { get; set; }
    public GameObject CardPrefab { get; set; }
    public Image CardImage { get; set; }
    public Image CountImage { get; set; }
    public int Amount { get; set; }
}
