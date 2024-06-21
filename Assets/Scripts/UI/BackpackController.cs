using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using UnityEngine.UI;

public class BackpackController : MonoBehaviour
{
    public GameObject seedInventoryCanvas; // Das Canvas, das ein- und ausgeblendet werden soll
    private CustomInputs input; // Referenz zum neuen Input System
    private bool isCanvasVisible = false; // Zustand des Canvas
    private int currentSelectionIndex = 0; // Index der aktuell ausgew?hlten Samenkarte
    private List<Image> seedCards = new List<Image>(); // Liste der Samenkarte

    private Backpack backpack;
    private PlayerMovement playerMovement;
    private Animator animator;

    private SeedInventory seedInventory;

    void Awake()
    {
        input = new CustomInputs();
    }

    void OnEnable()
    {
        input.Enable();
        input.HUD.BackpackButton.performed += ToggleCanvasVisibility; // Registriere die Eingabeaktion
        Debug.Log("Rucksack Input aktiviert");
    }

    void OnDisable()
    {
        input.HUD.BackpackButton.performed -= ToggleCanvasVisibility; // Deregistriere die Eingabeaktion
        input.Disable();
        Debug.Log("Rucksack Input deaktiviert");
    }

    void Start()
    {
        playerMovement = FindObjectOfType<PlayerMovement>();
        backpack = FindObjectOfType<Backpack>();
        seedInventory = FindObjectOfType<SeedInventory>();

        if (seedInventory == null)
        {
            Debug.LogError("No SeedInventory found in the scene.");
            return;
        }

        // Initialisiere die Samenkarte-Liste aus dem SeedInventory
        AddCardIfNotEmpty(seedInventory.farnCard, Seed.SeedType.Farn);
        AddCardIfNotEmpty(seedInventory.mangroveCard, Seed.SeedType.Mangrove);
        AddCardIfNotEmpty(seedInventory.crotonCard, Seed.SeedType.Croton);
        AddCardIfNotEmpty(seedInventory.alocasiaCard, Seed.SeedType.Alocasia);
        AddCardIfNotEmpty(seedInventory.teaktreeCard, Seed.SeedType.Teaktree);

        // Stelle sicher, dass das Canvas standardm??ig deaktiviert ist
        seedInventoryCanvas.SetActive(isCanvasVisible);
        Debug.Log("Start: Seed inventory canvas set to inactive.");

        // Initiales Update der Sichtbarkeit
        UpdateCardVisibility();

        // Initialisiere den Animator vom Kindobjekt
        if (playerMovement != null)
        {
            animator = playerMovement.GetComponentInChildren<Animator>();
            if (animator == null)
            {
                Debug.LogError("No Animator component found in children of PlayerMovement.");
            }
        }
    }

    public void RefreshSeedCards()
    {
        seedCards.Clear();
        AddCardIfNotEmpty(seedInventory.farnCard, Seed.SeedType.Farn);
        AddCardIfNotEmpty(seedInventory.mangroveCard, Seed.SeedType.Mangrove);
        AddCardIfNotEmpty(seedInventory.crotonCard, Seed.SeedType.Croton);
        AddCardIfNotEmpty(seedInventory.alocasiaCard, Seed.SeedType.Alocasia);
        AddCardIfNotEmpty(seedInventory.teaktreeCard, Seed.SeedType.Teaktree);

        UpdateCardVisibility();
    }

    private void AddCardIfNotEmpty(Image card, Seed.SeedType type)
    {
        if (backpack.GetSeedCountByType(type) > 0)
        {
            seedCards.Add(card);
        }
    }

    void ToggleCanvasVisibility(InputAction.CallbackContext context)
    {
        isCanvasVisible = !isCanvasVisible;
        seedInventoryCanvas.SetActive(isCanvasVisible); // Canvas ein- oder ausblenden

        if (isCanvasVisible)
        {
            input.Backpack.Move.performed += OnMove;
            input.Backpack.Select.performed += OnSelect;
            input.Backpack.Cancel.performed += OnCancel;
            playerMovement.DisableMovement();
        }
        else
        {
            input.Backpack.Move.performed -= OnMove;
            input.Backpack.Select.performed -= OnSelect;
            input.Backpack.Cancel.performed -= OnCancel;
            playerMovement.EnableMovement();
        }
    }

    private void OnMove(InputAction.CallbackContext context) // Navigieren durch Samenkarten
    {
        Debug.Log("OnMove");
        Debug.Log($"Anzahl der Samenkarten: {seedCards.Count}");

        if (seedCards.Count > 0)
        {
            Vector2 inputVector = context.ReadValue<Vector2>();
            if (inputVector.x > 0)
            {
                currentSelectionIndex = (currentSelectionIndex + 1) % seedCards.Count;
            }
            else if (inputVector.x < 0)
            {
                currentSelectionIndex = (currentSelectionIndex - 1 + seedCards.Count) % seedCards.Count;
            }
            Debug.Log("OnMove > doing update");

            UpdateCardVisibility();
        }
    }


    private void UpdateCardVisibility()
    {
        if (seedCards.Count > 0)
        {
            for (int i = 0; i < seedCards.Count; i++)
            {
                Color color = seedCards[i].color;
                color.a = (i == currentSelectionIndex) ? 1f : 0.5f;
                seedCards[i].color = color;
                Debug.Log($"seedCard color: {seedCards[i]} - {color}");
            }
        }
        else
        {
            Debug.Log("Keine Samenkarten vorhanden");
        }
    }


    // Logik zum Einpflanzen des ausgew?hlten Samens
    private void OnSelect(InputAction.CallbackContext context)
    {
        Debug.Log("Selected seed card: " + currentSelectionIndex);

        HandlePlanting();
        ToggleCanvasVisibility(context); // Schlie?en Sie den Rucksack nach der Auswahl
    }

    private void OnCancel(InputAction.CallbackContext context)
    {
        ToggleCanvasVisibility(context); // Schlie?en Sie den Rucksack ohne Aktion
    }

    private void HandlePlanting()
    {
        if (backpack != null && backpack.GetSeedCount() > 0) // ?berpr?fen Sie, ob Samen verf?gbar sind
        {
            if (playerMovement != null && animator != null)
            {
                playerMovement.SetCurrentSeedIndex(currentSelectionIndex); // Setzen des aktuellen Samenindex im PlayerMovement
                animator.SetTrigger("HandleGoPlant"); // Starten der Pflanzanimation
            }

            // Zeige den Samen in Pittis Hand w?hrend der Animation
            // seedInHand = Instantiate(seedPrefab, frontHandPosition.position, Quaternion.identity);
            // seedInHand.transform.SetParent(frontHandPosition);
            // seedInHand.SetActive(true);
        }
        else
        {
            Debug.Log("No seeds available to plant or Player input disabled.");
        }
    }

}
