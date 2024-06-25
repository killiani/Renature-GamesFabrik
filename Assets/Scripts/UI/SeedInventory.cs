using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SeedInventory : MonoBehaviour
{
    public Transform cardContainer; // Container fuer die Karten
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
    private Dictionary<Seed.SeedType, int> prevSeedCounts = new Dictionary<Seed.SeedType, int>();

    void Start()
    {
        backpack = FindObjectOfType<Backpack>();
        UpdateSeedDisplay(); // Initiales Update
    }

    void Update()
    {
        // Nur aktualisieren, wenn sich etwas geaendert hat
        if (HasSeedsChanged())
        {
            UpdateSeedDisplay();
        }
    }

    private bool HasSeedsChanged()
    {
        Dictionary<Seed.SeedType, int> currentSeedCounts = new Dictionary<Seed.SeedType, int>();

        // Samenanzahl basierend auf dem Rucksack aktualisieren
        foreach (Seed seed in backpack.GetAllSeeds())
        {
            if (!currentSeedCounts.ContainsKey(seed.Type))
            {
                currentSeedCounts[seed.Type] = 0;
            }
            currentSeedCounts[seed.Type]++;
        }

        // prueffen, ob sich die Samenanzahl geandert hat
        bool hasChanged = false;
        foreach (Seed.SeedType seedType in System.Enum.GetValues(typeof(Seed.SeedType)))
        {
            int prevCount = prevSeedCounts.ContainsKey(seedType) ? prevSeedCounts[seedType] : 0;
            int currentCount = currentSeedCounts.ContainsKey(seedType) ? currentSeedCounts[seedType] : 0;

            if (prevCount != currentCount)
            {
                prevSeedCounts[seedType] = currentCount;
                hasChanged = true;
            }
        }

        return hasChanged;
    }

    // Update visibility and count for the plants
    private void UpdateSeedDisplay()
    {
        // Setze alle Karten auf unsichtbar
        SetCardVisibility(farnCard, farnCount, false);
        SetCardVisibility(mangroveCard, mangroveCount, false);
        SetCardVisibility(crotonCard, crotonCount, false);
        SetCardVisibility(alocasiaCard, alocasiaCount, false);
        SetCardVisibility(teaktreeCard, teaktreeCount, false);

        // Zeige die Karten basierend auf dem aktuellen Zustand
        foreach (var seedCount in prevSeedCounts)
        {
            if (seedCount.Value > 0)
            {
                ShowCard(seedCount.Key, seedCount.Value);
            }
        }
    }

    private void SetCardVisibility(Image card, Image countImage, bool visible)
    {
        card.gameObject.SetActive(visible);
        countImage.gameObject.SetActive(visible);
    }

    private void ShowCard(Seed.SeedType seedType, int count)
    {
        Image card = null;
        Image countImage = null;

        switch (seedType)
        {
            case Seed.SeedType.Farn:
                card = farnCard;
                countImage = farnCount;
                break;
            case Seed.SeedType.Mangrove:
                card = mangroveCard;
                countImage = mangroveCount;
                break;
            case Seed.SeedType.Croton:
                card = crotonCard;
                countImage = crotonCount;
                break;
            case Seed.SeedType.Alocasia:
                card = alocasiaCard;
                countImage = alocasiaCount;
                break;
            case Seed.SeedType.Teaktree:
                card = teaktreeCard;
                countImage = teaktreeCount;
                break;
        }

        if (card != null && countImage != null)
        {
            card.transform.SetParent(cardContainer, false);
            SetCardVisibility(card, countImage, true);
            countImage.sprite = GetNumberSprite(count);
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
