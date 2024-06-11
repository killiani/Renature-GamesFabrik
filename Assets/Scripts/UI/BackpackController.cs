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
    private List<Image> seedCards = new List<Image>(); // Liste der Samenkarte

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

        // Initialisiere die Samenkarte-Liste aus dem SeedInventory
        seedCards.Add(seedInventory.farnCard);
        seedCards.Add(seedInventory.mangroveCard);
        seedCards.Add(seedInventory.crotonCard);
        seedCards.Add(seedInventory.alocasiaCard);
        seedCards.Add(seedInventory.teaktreeCard);

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



    void ToggleCanvasVisibility(InputAction.CallbackContext context)
    {
        isCanvasVisible = !isCanvasVisible;
        seedInventoryCanvas.SetActive(isCanvasVisible); // Canvas ein- oder ausblenden

        if (isCanvasVisible)
        {
            Debug.Log("Backpack opened, setting up input actions.");
            input.Backpack.Move.performed += OnMove;
            input.Backpack.Select.performed += OnSelect;
            input.Backpack.Cancel.performed += OnCancel;
            playerMovement.DisableMovement();
        }
        else
        {
            Debug.Log("Backpack closed, tearing down input actions.");
            input.Backpack.Move.performed -= OnMove;
            input.Backpack.Select.performed -= OnSelect;
            input.Backpack.Cancel.performed -= OnCancel;
            playerMovement.EnableMovement();
        }

        Debug.Log("ToggleCanvasVisibility: Canvas visibility toggled.");
    }

    private void OnMove(InputAction.CallbackContext context)
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
            Color color = seedCards[i].color;
            color.a = (i == currentSelectionIndex) ? 1f : 0.5f;
            seedCards[i].color = color;
        }
    }

    // Logik zum Einpflanzen des ausgewählten Samens
    private void OnSelect(InputAction.CallbackContext context)
    {
        Debug.Log("Selected seed card: " + currentSelectionIndex);
        HandlePlanting();
        ToggleCanvasVisibility(context); // Schließen Sie den Rucksack nach der Auswahl
    }

    private void OnCancel(InputAction.CallbackContext context)
    {
        ToggleCanvasVisibility(context); // Schließen Sie den Rucksack ohne Aktion
    }

    private void HandlePlanting()
    {
        if (backpack != null && backpack.GetSeedCount() > 0) // Überprüfen Sie, ob Samen verfügbar sind
        {
            if (playerMovement != null && animator != null)
            {
                playerMovement.SetCurrentSeedIndex(currentSelectionIndex); // Setzen des aktuellen Samenindex im PlayerMovement
                animator.SetTrigger("HandleGoPlant"); // Starten der Pflanzanimation
            }

            // Zeige den Samen in Pittis Hand während der Animation
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
