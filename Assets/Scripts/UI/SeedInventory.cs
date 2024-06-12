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

    private void InitializeSeedCards()
    {
        seedCards.Add(new SeedCard { SeedType = Seed.SeedType.Farn, CardPrefab = farnCardPrefab });
        seedCards.Add(new SeedCard { SeedType = Seed.SeedType.Mangrove, CardPrefab = mangroveCardPrefab });
        seedCards.Add(new SeedCard { SeedType = Seed.SeedType.Croton, CardPrefab = crotonCardPrefab });
        seedCards.Add(new SeedCard { SeedType = Seed.SeedType.Alocasia, CardPrefab = alocasiaCardPrefab });
        seedCards.Add(new SeedCard { SeedType = Seed.SeedType.Teaktree, CardPrefab = teaktreeCardPrefab });

        // Instantiate the prefabs and get their UI components
        foreach (var seedCard in seedCards)
        {
            GameObject cardInstance = Instantiate(seedCard.CardPrefab, transform);
            seedCard.CardImage = cardInstance.GetComponent<Image>();
            seedCard.CountImage = cardInstance.transform.GetChild(0).GetComponent<Image>(); // Assuming the CountImage is the first child
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

    private void UpdateSeedDisplay()
    {
        int position = 0;
        foreach (var seedCard in seedCards)
        {
            if (seedCard.Amount > 0)
            {
                seedCard.CardImage.gameObject.SetActive(true);
                seedCard.CountImage.sprite = GetNumberSprite(seedCard.Amount);
                seedCard.CountImage.gameObject.SetActive(true);
                // Set position based on the index
                seedCard.CardImage.rectTransform.anchoredPosition = new Vector2(position * 120, 0);
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
