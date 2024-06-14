using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using UnityEngine.UI;

public class BackpackController : MonoBehaviour
{
    public GameObject seedInventoryCanvas; // Das Canvas, das ein- und ausgeblendet werden soll
    private CustomInputs input; // Referenz zum neuen Input System
    private bool isCanvasVisible = false; // Zustand des Canvas
    private int currentSelectionIndex = 0; // Index der aktuell ausgewählten Samenkarte
    private List<SeedCard> seedCards; // Liste der Samenkarte

    private Backpack backpack;
    private PlayerMovement playerMovement;
    private Animator animator;
    private SeedInventory seedInventory;

    void Awake()
    {
        input = new CustomInputs();
        Debug.Log("Awake: CustomInputs initialized.");
    }

    void OnEnable()
    {
        input.Enable();
        input.HUD.BackpackButton.performed += ToggleCanvasVisibility; // Registriere die Eingabeaktion
        Debug.Log("OnEnable: Input actions enabled.");
    }

    void OnDisable()
    {
        input.HUD.BackpackButton.performed -= ToggleCanvasVisibility; // Deregistriere die Eingabeaktion
        input.Disable();
        Debug.Log("OnDisable: Input actions disabled.");
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
        if (backpack == null)
        {
            Debug.LogError("No Backpack found in the scene.");
            return;
        }

        seedInventory.InitializeSeedCards();
        seedInventory.UpdateSeedDisplay();
        seedCards = new List<SeedCard>(seedInventory.seedCards);
        if (seedCards.Count == 0)
        {
            Debug.LogError("Seed cards are not initialized correctly.");
        }

        seedInventoryCanvas.SetActive(isCanvasVisible);
        Debug.Log("Start: Seed inventory canvas set to inactive.");

        currentSelectionIndex = 0;
        seedInventory.UpdateSeedDisplay();
        UpdateCardVisibility();

        if (playerMovement != null)
        {
            animator = playerMovement.GetComponentInChildren<Animator>();
        }
    }

    void ToggleCanvasVisibility(InputAction.CallbackContext context)
    {
        isCanvasVisible = !isCanvasVisible;
        seedInventoryCanvas.SetActive(isCanvasVisible); // Canvas ein- oder ausblenden

        if (isCanvasVisible)
        {
            Debug.Log($"Current selection index: {currentSelectionIndex}");
            input.Backpack.Move.performed += OnMove;
            input.Backpack.Select.performed += OnSelect;
            input.Backpack.Cancel.performed += OnCancel;
            playerMovement.DisableMovement();

            seedInventory.InitializeSeedCards(); // Karten neu initialisieren
            seedInventory.UpdateSeedDisplay();
            FilterActiveSeedCards(); // Filtere die Karten beim Öffnen des Canvas
            seedInventory.UpdateSeedDisplay();
            currentSelectionIndex = 0; // Ensure selection index is reset
            UpdateCardVisibility();
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
        if (seedCards == null || seedCards.Count == 0)
        {
            Debug.LogError("No seed cards available.");
            return;
        }

        Debug.Log($"Current selection index before move: {currentSelectionIndex}");
        Vector2 inputVector = context.ReadValue<Vector2>();
        if (inputVector.x > 0)
        {
            currentSelectionIndex = (currentSelectionIndex + 1) % seedCards.Count;
        }
        else if (inputVector.x < 0)
        {
            currentSelectionIndex = (currentSelectionIndex - 1 + seedCards.Count) % seedCards.Count;
        }
        Debug.Log($"Current selection index after move: {currentSelectionIndex}");

        UpdateCardVisibility();
    }


    private void UpdateCardVisibility()
    {
        Debug.Log("UpdateCardVisibility called");
        for (int i = 0; i < seedCards.Count; i++)
        {
            Color color = seedCards[i].CardImage.color;
            color.a = (i == currentSelectionIndex) ? 1f : 0.5f;
            seedCards[i].CardImage.color = color;
            Debug.Log($"Card {i} visibility updated, alpha: {color.a}");
        }
    }

    private void OnSelect(InputAction.CallbackContext context)
    {
        if (currentSelectionIndex >= 0 && currentSelectionIndex < seedCards.Count)
        {
            Debug.Log("Selected seed card: " + currentSelectionIndex);

            HandlePlanting();
            ToggleCanvasVisibility(context); // Schließen Sie den Rucksack nach der Auswahl
        }
    }

    private void OnCancel(InputAction.CallbackContext context)
    {
        ToggleCanvasVisibility(context); // Schließen Sie den Rucksack ohne Aktion
    }

    private void HandlePlanting()
    {
        if (backpack != null && backpack.GetSeedCount(seedCards[currentSelectionIndex].SeedType) > 0) // Überprüfen Sie, ob Samen verfügbar sind
        {
            if (playerMovement != null && animator != null)
            {
                playerMovement.SetCurrentSeedIndex(currentSelectionIndex);
                animator.SetTrigger("HandleGoPlant"); // Starten der Pflanzanimation
            }
        }
        else
        {
            Debug.Log("No seeds available to plant or Player input disabled.");
        }
    }

    private void FilterActiveSeedCards()
    {
        seedCards = seedInventory.seedCards.FindAll(card => card.Amount > 0);
        Debug.Log($"Active seed cards count: {seedCards.Count}");
        foreach (var card in seedCards)
        {
            Debug.Log($"Active card: {card.SeedType}, Amount: {card.Amount}");
        }
        if (seedCards.Count == 0)
        {
            Debug.LogError("No active seed cards available.");
        }
    }

}
