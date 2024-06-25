using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameMenu : MonoBehaviour
{
    public GameObject menuPanel; // Das Menü-Panel im Unity-Editor.
    private CustomInputs input;
    private PlayerMovement playerMovement;
    public Button continueButton;
    public Button exitButton;

    void Awake()
    {
        input = new CustomInputs();
        input.HUD.PauseMenu.performed += ToggleMenu;
        input.MenuPause.PrimaryAction.performed += PerformPrimaryAction;
    }

    void Start()
    {
        menuPanel.SetActive(false); // Deaktiviere das Menü-Panel beim Start
        playerMovement = FindObjectOfType<PlayerMovement>(); // Findet die PlayerMovement Komponente im Spiel
        if (playerMovement == null)
        {
            Debug.LogError("PlayerMovement component not found on any game objects in the scene.");
        }
        DisableInputs();
    }

    void OnEnable()
    {
        input.Enable(); // Aktiviere alle Input-Listener
        input.MenuPause.Move.performed += NavigateMenu;
        input.MenuPause.PrimaryAction.performed += PerformPrimaryAction;
    }

    void OnDisable()
    {
        input.Disable(); // Deaktiviere alle Input-Listener
        input.MenuPause.Move.performed -= NavigateMenu;
        input.MenuPause.PrimaryAction.performed -= PerformPrimaryAction;
    }

    private void DisableInputs()
    {
        input.MenuPause.PrimaryAction.performed -= PerformPrimaryAction;
    }

    private void EnableInputs()
    {
        input.MenuPause.PrimaryAction.performed += PerformPrimaryAction;
    }


    // Methode zum Umschalten des Menüs
    private void ToggleMenu(InputAction.CallbackContext context)
    {
        bool isActive = !menuPanel.activeSelf;
        menuPanel.SetActive(isActive);
        Time.timeScale = isActive ? 0 : 1;  // Pausiert oder setzt das Spiel fort, basierend auf dem Menüstatus

        if (isActive)
        {
            EnableInputs();
            playerMovement.DisableMovement();
            EventSystem.current.SetSelectedGameObject(null);  // Zuerst das aktuell gewählte Objekt zurücksetzen
            Button firstButton = menuPanel.GetComponentInChildren<Button>();
            if (firstButton != null)
            {
                EventSystem.current.SetSelectedGameObject(firstButton.gameObject);
            }
        }
        else
        {
            DisableInputs();
            playerMovement.EnableMovement();
        }
    }


    // Navigiert zwischen den Buttons im Menü
    private void NavigateMenu(InputAction.CallbackContext context)
    {
        Vector2 moveInput = context.ReadValue<Vector2>();
        GameObject currentSelected = UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject;
        Selectable currentSelectable = currentSelected?.GetComponent<Selectable>();
        if (currentSelectable == null) return;

        Selectable nextSelectable = null;
        if (moveInput.y > 0)
        {
            //nextSelectable = currentSelectable.FindSelectableOnUp();
            EventSystem.current.SetSelectedGameObject(continueButton.gameObject);
        }
        else if (moveInput.y < 0)
        {
            //nextSelectable = currentSelectable.FindSelectableOnDown();
            EventSystem.current.SetSelectedGameObject(exitButton.gameObject);
        }

        if (nextSelectable != null)
        {
            UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(nextSelectable.gameObject);
        }
    }

    // Führt die primäre Aktion des aktuell fokussierten Buttons aus
    private void PerformPrimaryAction(InputAction.CallbackContext context)
    {
        Debug.Log("Primary action performed");
        var currentButton = UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject?.GetComponent<Button>();
        currentButton?.onClick.Invoke();
        // Möglicherweise Bewegung wieder aktivieren, wenn das Menü geschlossen wird
    }

    // Schließt das Menü und setzt das Spiel fort
    public void ResumeGame()
    {
        menuPanel.SetActive(false);
        Time.timeScale = 1; // Setzt das Spiel fort
        playerMovement.EnableMovement();
    }

    // Lädt das Hauptmenü
    public void GoToMainMenu()
    {
        SceneManager.LoadScene("StartScreen"); // Lädt die Hauptmenüszene
        Time.timeScale = 1; // Stellt sicher, dass das Spiel nicht pausiert bleibt
    }
}
