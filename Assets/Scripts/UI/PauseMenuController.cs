using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameMenu : MonoBehaviour
{
    public GameObject menuPanel; // Das Menü-Panel im Unity-Editor.
    private CustomInputs input;

    void Awake()
    {
        input = new CustomInputs();
        input.HUD.PauseMenu.performed += ToggleMenu;
        input.MenuPause.PrimaryAction.performed += PerformPrimaryAction;
    }

    void Start()
    {
        menuPanel.SetActive(false); // Deaktiviere das Menü-Panel beim Start
    }


    void OnEnable()
    {
        //input.Enable(); // Aktiviere alle Input-Listener
        input.MenuPause.Move.performed += NavigateMenu;
        input.MenuPause.PrimaryAction.performed += PerformPrimaryAction;
    }

    void OnDisable()
    {
        //input.Disable(); // Deaktiviere alle Input-Listener
        input.MenuPause.Move.performed -= NavigateMenu;
        input.MenuPause.PrimaryAction.performed -= PerformPrimaryAction;
    }

    // Methode zum Umschalten des Menüs
    private void ToggleMenu(InputAction.CallbackContext context)
    {
        Debug.Log("Menu Open");
        menuPanel.SetActive(!menuPanel.activeSelf);
        Time.timeScale = menuPanel.activeSelf ? 0 : 1; // Pausiert das Spiel, wenn das Menü aktiv ist

        if (menuPanel.activeSelf)
        {
            Button firstButton = menuPanel.GetComponentInChildren<Button>();
            if (firstButton != null)
            {
                UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(firstButton.gameObject);
            }
        }
    }

    private void NavigateMenu(InputAction.CallbackContext context)
    {
        Vector2 moveInput = context.ReadValue<Vector2>();
        GameObject currentSelected = UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject;
        Selectable currentSelectable = currentSelected?.GetComponent<Selectable>();
        if (currentSelectable == null) return;

        Selectable nextSelectable = null;
        if (moveInput.y > 0)
        {
            nextSelectable = currentSelectable.FindSelectableOnUp();
        }
        else if (moveInput.y < 0)
        {
            nextSelectable = currentSelectable.FindSelectableOnDown();
        }

        if (nextSelectable != null)
        {
            UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(nextSelectable.gameObject);
        }
    }


    private void PerformPrimaryAction(InputAction.CallbackContext context)
    {
        Debug.Log("Primary action performed");
        UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject?.GetComponent<Button>()?.onClick.Invoke();
    }

    public void ResumeGame()
    {
        menuPanel.SetActive(false);
        Time.timeScale = 1; // Setzt das Spiel fort
    }

    public void GoToMainMenu()
    {
        SceneManager.LoadScene("StartScreen"); // Ersetze "MainMenu" mit dem Namen deiner Hauptmenüszene
        Time.timeScale = 1; // Stellt sicher, dass das Spiel nicht pausiert bleibt
    }
}
