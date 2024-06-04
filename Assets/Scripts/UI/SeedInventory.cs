using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SeedInventory : MonoBehaviour
{
    public Image farnCard;
    public Image mangroveCard;
    public Image crotonCard;
    public Image alocasiaCard;

    public Image farnCount;
    public Image mangroveCount;
    public Image crotonCount;
    public Image alocasiaCount;

    public Sprite[] numberSprites; // Array der Zahlensprites

    private Backpack backpack;

    // Variablen zum Speichern des vorherigen Zustands
    private int prevFarnSeeds = 0;
    private int prevMangroveSeeds = 0;
    private int prevCrotonSeeds = 0;
    private int prevAlocasiaSeeds = 0;

    void Start()
    {
        backpack = FindObjectOfType<Backpack>();
        UpdateSeedDisplay(); // Initiales Update
    }

    void Update()
    {
        // Nur aktualisieren, wenn sich etwas geändert hat
        if (HasSeedsChanged())
        {
            UpdateSeedDisplay();
        }
    }

    private bool HasSeedsChanged()
    {
        int farnSeeds = 0;
        int mangroveSeeds = 0;
        int crotonSeeds = 0;
        int alocasiaSeeds = 0;

        // Samenanzahl basierend auf dem Rucksack aktualisieren
        foreach (Seed seed in backpack.GetAllSeeds())
        {
            switch (seed.Type)
            {
                case Seed.SeedType.Farn:
                    farnSeeds++;
                    break;
                case Seed.SeedType.Mangrove:
                    mangroveSeeds++;
                    break;
                case Seed.SeedType.Croton:
                    crotonSeeds++;
                    break;
                case Seed.SeedType.Alocasia:
                    alocasiaSeeds++;
                    break;
            }
        }

        // Überprüfen, ob sich die Samenanzahl geändert hat
        if (farnSeeds != prevFarnSeeds || mangroveSeeds != prevMangroveSeeds || crotonSeeds != prevCrotonSeeds || alocasiaSeeds != prevAlocasiaSeeds)
        {
            prevFarnSeeds = farnSeeds;
            prevMangroveSeeds = mangroveSeeds;
            prevCrotonSeeds = crotonSeeds;
            prevAlocasiaSeeds = alocasiaSeeds;
            return true;
        }
        return false;
    }

    private void UpdateSeedDisplay()
    {
        // Update visibility and count for Farn
        UpdateCard(farnCard, farnCount, prevFarnSeeds);

        // Update visibility and count for Mangrove
        UpdateCard(mangroveCard, mangroveCount, prevMangroveSeeds);

        // Update visibility and count for Croton
        UpdateCard(crotonCard, crotonCount, prevCrotonSeeds);

        // Update visibility and count for Alocasia
        UpdateCard(alocasiaCard, alocasiaCount, prevAlocasiaSeeds);
    }

    private void UpdateCard(Image card, Image countImage, int count)
    {
        if (count > 0)
        {
            card.gameObject.SetActive(true);
            countImage.sprite = GetNumberSprite(count);
            countImage.gameObject.SetActive(true);
        }
        else
        {
            card.gameObject.SetActive(false);
            countImage.gameObject.SetActive(false);
        }
    }

    private Sprite GetNumberSprite(int number)
    {
        if (number >= 0 && number < numberSprites.Length)
        {
            return numberSprites[number];
        }
        return null;
    }
}
