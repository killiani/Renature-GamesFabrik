using UnityEngine;
using UnityEngine.InputSystem;

public class BackpackController : MonoBehaviour
{
    public GameObject seedInventoryCanvas; // Das Canvas, das ein- und ausgeblendet werden soll

    private CustomInputs input; // Referenz zum neuen Input System
    private bool isCanvasVisible = false; // Zustand des Canvas

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
        // Stelle sicher, dass das Canvas standardm‰ﬂig deaktiviert ist
        seedInventoryCanvas.SetActive(isCanvasVisible);
        Debug.Log("Start: Seed inventory canvas set to inactive.");
    }

    void ToggleCanvasVisibility(InputAction.CallbackContext context)
    {
        isCanvasVisible = !isCanvasVisible;
        seedInventoryCanvas.SetActive(isCanvasVisible); // Canvas ein- oder ausblenden
        Debug.Log("ToggleCanvasVisibility: Canvas visibility toggled.");
    }
}
