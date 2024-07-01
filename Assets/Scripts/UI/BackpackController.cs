using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using UnityEngine.UI;

public class BackpackController : MonoBehaviour
{
    public GameObject seedInventoryCanvas; // Das Canvas, das ein- und ausgeblendet werden soll
    private CustomInputs input; // Referenz zum neuen Input System
    private bool isCanvasVisible = false; // Zustand des Canvas
    private int currentSelectionIndex = 0; // Index der aktuell ausgewaehlten Samenkarte
    private string currentSelectedSeed = "";
    private List<Image> seedCards = new List<Image>(); // Liste der Samenkarte
    private float inputCooldown = 0.2f; // Abkühlzeit zwischen den Eingaben in Sekunden
    private float lastInputTime = 0f; // Zeitpunkt der letzten Eingabe

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
        input.HUD.BackpackButton.performed += OnBackpackButtonPressed; // Registriere die Eingabeaktion
        Debug.Log("Rucksack Input aktiviert");
    }

    void OnDisable()
    {
        input.Disable();
        input.HUD.BackpackButton.performed -= OnBackpackButtonPressed; // Deregistriere die Eingabeaktion
        Debug.Log("Rucksack Input deaktiviert");
    }

    public void DisableBackpack()
    {
        input.HUD.BackpackButton.performed -= OnBackpackButtonPressed;
    }

    public void EnableBackpack()
    {
        input.HUD.BackpackButton.performed += OnBackpackButtonPressed;
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

        // Stelle sicher, dass das Canvas standardmäßig deaktiviert ist
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

    public void SortSeedCards()
    {
        seedCards.Sort((a, b) => a.transform.GetSiblingIndex().CompareTo(b.transform.GetSiblingIndex()));
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

    void OnBackpackButtonPressed(InputAction.CallbackContext context)
    {
        // Toggle canvas visibility
        ToggleCanvasVisibility();

        // Show or hide blocks based on the canvas visibility
        if (isCanvasVisible)
        {
            playerMovement.ShowAllBlocks();
        }
        else
        {
            playerMovement.HideAllBlocks();
        }
    }

    void ToggleCanvasVisibility()
    {
        isCanvasVisible = !isCanvasVisible;
        seedInventoryCanvas.SetActive(isCanvasVisible); // Canvas ein- oder ausblenden

        if (isCanvasVisible)
        {
            SortSeedCards();
            input.Backpack.Move.performed += OnMove;
            input.Backpack.Select.performed += OnSelect;
            input.Backpack.Cancel.performed += OnCancel;
            playerMovement.DisableMovement();
            playerMovement.DisableNightAction();
        }
        else
        {
            input.Backpack.Move.performed -= OnMove;
            input.Backpack.Select.performed -= OnSelect;
            input.Backpack.Cancel.performed -= OnCancel;
            playerMovement.EnableMovement();
            playerMovement.EnableNightAction();
        }
    }

    public void ShowAllBlocks()
    {
        playerMovement.ShowAllBlocks();
    }

    public void HideAllBlocks()
    {
        playerMovement.HideAllBlocks();
    }

    private void OnMove(InputAction.CallbackContext context) // Navigieren durch Samenkarten
    {
        if (Time.time - lastInputTime < inputCooldown)
        {
            // Eingabe ignorieren, wenn die Abkühlzeit noch nicht abgelaufen ist
            return;
        }

        Debug.Log("OnMove");
        // Debug.Log($"Anzahl der Samenkarten: {seedCards.Count}");

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

            // Aktualisiere den Zeitpunkt der letzten Eingabe
            lastInputTime = Time.time;
        }
    }

    private void UpdateCardVisibility()
    {
        SortSeedCards();
        if (seedCards.Count > 0)
        {
            for (int i = 0; i < seedCards.Count; i++)
            {
                // Setze die Transparenz der Karte basierend auf der aktuellen Auswahl
                Color color = seedCards[i].color;
                color.a = (i == currentSelectionIndex) ? 1f : 0.5f;
                seedCards[i].color = color;

                if (i == currentSelectionIndex)
                {
                    currentSelectedSeed = seedCards[i].name;
                    // Debug.Log("Selected: "+ seedCards[i].name);
                }

                // Debug.Log($"seedCard color: {seedCards[i].name} - {color}");
            }
        }
        else
        {
            Debug.Log("Keine Samenkarten vorhanden");
        }
    }

    // Logik zum Einpflanzen des ausgewaehlten Samens
    private void OnSelect(InputAction.CallbackContext context)
    {
        // Debug.Log("Selected seed card Number: " + currentSelectionIndex+ " - Name: " + seedCards[currentSelectionIndex].name);
        HandlePlanting();
        ToggleCanvasVisibility(); // Schließt Rucksack nach Auswahl
    }

    private void OnCancel(InputAction.CallbackContext context)
    {
        ToggleCanvasVisibility(); // Schließt Rucksack ohne Auswahl
    }

    private void HandlePlanting()
    {
        if (backpack != null && backpack.GetSeedCount() > 0) // Überprüfen ob Samen verfügbar sind
        {
            if (playerMovement != null && animator != null) // TODO: vereinfachen
            {
                playerMovement.DisableMovement(); // wird am ende der Holding Animation wieder aktiviert
                playerMovement.HoldSeedAndReadyToPlant(currentSelectionIndex, currentSelectedSeed);
            }
        }
        else
        {
            Debug.Log("No seeds available to plant or Player input disabled.");
        }
    }
}
