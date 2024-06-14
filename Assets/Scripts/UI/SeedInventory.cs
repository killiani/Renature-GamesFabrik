using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SeedInventory : MonoBehaviour
{
    public List<SeedCard> seedCards = new List<SeedCard>();
    public Sprite[] numberSprites; // Array der Zahlensprites

    public GameObject farnCardPrefab;
    public GameObject mangroveCardPrefab;
    public GameObject crotonCardPrefab;
    public GameObject alocasiaCardPrefab;
    public GameObject teaktreeCardPrefab;

    private Backpack backpack;

    void Start()
    {
        backpack = FindObjectOfType<Backpack>();
        if (backpack == null)
        {
            Debug.LogError("No Backpack found in the scene.");
            return;
        }
        InitializeSeedCards();
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

    public void InitializeSeedCards()
    {

        if (backpack == null)
        {
            Debug.LogError("Backpack reference is null.");
            return;
        }

        seedCards.Clear(); // Leere die Liste, um doppelte Einträge zu vermeiden

        var seedTypes = new Dictionary<Seed.SeedType, GameObject>
    {
        { Seed.SeedType.Farn, farnCardPrefab },
        { Seed.SeedType.Mangrove, mangroveCardPrefab },
        { Seed.SeedType.Croton, crotonCardPrefab },
        { Seed.SeedType.Alocasia, alocasiaCardPrefab },
        { Seed.SeedType.Teaktree, teaktreeCardPrefab }
    };

        foreach (var seedType in seedTypes)
        {
            int amount = backpack.GetSeedCount(seedType.Key);
            if (amount > 0)
            {
                var seedCard = new SeedCard { SeedType = seedType.Key, CardPrefab = seedType.Value, Amount = amount };
                GameObject cardInstance = Instantiate(seedCard.CardPrefab, transform);
                seedCard.CardImage = cardInstance.GetComponent<Image>();
                seedCard.CountImage = cardInstance.transform.GetChild(0).GetComponent<Image>(); // Assuming the CountImage is the first child
                seedCard.CardImage.gameObject.SetActive(false); // Ensure it's initially inactive
                seedCards.Add(seedCard);
            }
        }
    }


    private bool HasSeedsChanged()
    {
        bool hasChanged = false;
        foreach (var seedCard in seedCards)
        {
            int currentCount = backpack.GetSeedCount(seedCard.SeedType);
            if (seedCard.Amount != currentCount)
            {
                seedCard.Amount = currentCount;
                hasChanged = true;
            }
        }
        return hasChanged;
    }

    public void UpdateSeedDisplay()
    {
        Debug.Log("UpdateSeedDisplay called");
        int activeCardCount = 0;
        foreach (var seedCard in seedCards)
        {
            if (seedCard.Amount > 0)
            {
                activeCardCount++;
            }
        }

        float cardWidth = 240f; // Breite einer Karte
        float spacing = 20f; // Abstand zwischen den Karten
        float totalWidth = activeCardCount * (cardWidth + spacing) - spacing; // Gesamte Breite der Karten
        float startX = -totalWidth / 2 + cardWidth / 2; // Startposition für die mittige Ausrichtung

        int position = 0;
        foreach (var seedCard in seedCards)
        {
            if (seedCard.Amount > 0)
            {
                Debug.Log($"Setting position for {seedCard.SeedType}");
                seedCard.CardImage.gameObject.SetActive(true);
                seedCard.CountImage.sprite = GetNumberSprite(seedCard.Amount);
                seedCard.CountImage.gameObject.SetActive(true);

                // Setze die Position basierend auf dem Index und der berechneten Startposition
                float xPos = startX + position * (cardWidth + spacing);
                seedCard.CardImage.rectTransform.anchoredPosition = new Vector2(xPos, 0);
                position++;
            }
            else
            {
                seedCard.CardImage.gameObject.SetActive(false);
                seedCard.CountImage.gameObject.SetActive(false);
            }
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
