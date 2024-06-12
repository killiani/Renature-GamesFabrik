using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using UnityEngine.UI;

public class BackpackController : MonoBehaviour
{
    public GameObject seedInventoryCanvas; // Das Canvas, das ein- und ausgeblendet werden soll
    private CustomInputs input; // Referenz zum neuen Input System
    private bool isCanvasVisible = false; // Zustand des Canvas
    private int currentSelectionIndex = 0; // Index der aktuell ausgew�hlten Samenkarte
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

        seedCards = new List<SeedCard>(seedInventory.seedCards);

        seedInventoryCanvas.SetActive(isCanvasVisible);
        Debug.Log("Start: Seed inventory canvas set to inactive.");

        UpdateCardVisibility();

        if (playerMovement != null)
        {
            animator = playerMovement.GetComponentInChildren<Animator>();
            if (animator == null)
            {
                Debug.LogError("No Animator component found in children of PlayerMovement.");
            }
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
            FilterActiveSeedCards(); // Filtere die Karten beim �ffnen des Canvas
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
        Vector2 inputVector = context.ReadValue<Vector2>();
        if (inputVector.x > 0)
        {
            currentSelectionIndex = (currentSelectionIndex + 1) % seedCards.Count;
        }
        else if (inputVector.x < 0)
        {
            currentSelectionIndex = (currentSelectionIndex - 1 + seedCards.Count) % seedCards.Count;
        }

        UpdateCardVisibility();
    }

    private void UpdateCardVisibility()
    {
        for (int i = 0; i < seedCards.Count; i++)
        {
            Color color = seedCards[i].CardImage.color;
            color.a = (i == currentSelectionIndex) ? 1f : 0.5f;
            seedCards[i].CardImage.color = color;
        }
    }

    private void OnSelect(InputAction.CallbackContext context)
    {
        if (currentSelectionIndex >= 0 && currentSelectionIndex < seedCards.Count)
        {
            Debug.Log("Selected seed card: " + currentSelectionIndex);

            HandlePlanting();
            ToggleCanvasVisibility(context); // Schlie�en Sie den Rucksack nach der Auswahl
        }
    }

    private void OnCancel(InputAction.CallbackContext context)
    {
        ToggleCanvasVisibility(context); // Schlie�en Sie den Rucksack ohne Aktion
    }

    private void HandlePlanting()
    {
        if (backpack != null && backpack.GetSeedCount(seedCards[currentSelectionIndex].SeedType) > 0) // �berpr�fen Sie, ob Samen verf�gbar sind
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
    }
}
