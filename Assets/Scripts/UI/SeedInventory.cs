using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SeedInventory : MonoBehaviour
{
    public Image farnCard;
    public Image mangroveCard;
    public Image crotonCard;
    public Image alocasiaCard;
    public Image teaktreeCard;

    public Image farnCount;
    public Image mangroveCount;
    public Image crotonCount;
    public Image alocasiaCount;
    public Image teaktreeCount;

    public Sprite[] numberSprites; // Array der Zahlensprites

    private Backpack backpack;

    // Variablen zum Speichern des vorherigen Zustands
    private int prevFarnSeeds = 0;
    private int prevMangroveSeeds = 0;
    private int prevCrotonSeeds = 0;
    private int prevAlocasiaSeeds = 0;
    private int prevTeaktreeSeeds = 0;

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
        int teaktreeSeeds = 0;

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
                case Seed.SeedType.Teaktree:
                    teaktreeSeeds++;
                    break;
            }
        }

        // Überprüfen, ob sich die Samenanzahl geändert hat
        if (farnSeeds != prevFarnSeeds || 
            mangroveSeeds != prevMangroveSeeds ||
            crotonSeeds != prevCrotonSeeds || 
            alocasiaSeeds != prevAlocasiaSeeds || 
            teaktreeSeeds != prevTeaktreeSeeds)
        {
            prevFarnSeeds = farnSeeds;
            prevMangroveSeeds = mangroveSeeds;
            prevCrotonSeeds = crotonSeeds;
            prevAlocasiaSeeds = alocasiaSeeds;
            prevTeaktreeSeeds = teaktreeSeeds;
            return true;
        }
        return false;
    }

    // Update visibility and count for the plants
    private void UpdateSeedDisplay()
    {
        UpdateCard(farnCard, farnCount, prevFarnSeeds);
        UpdateCard(mangroveCard, mangroveCount, prevMangroveSeeds);
        UpdateCard(crotonCard, crotonCount, prevCrotonSeeds);
        UpdateCard(alocasiaCard, alocasiaCount, prevAlocasiaSeeds);
        UpdateCard(teaktreeCard, teaktreeCount, prevTeaktreeSeeds);
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
