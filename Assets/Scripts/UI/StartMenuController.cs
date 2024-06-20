using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class StartMenuController : MonoBehaviour
{
    public GameObject menuPanel;  // Das Panel, das die Menübuttons enthält

    private CustomInputs input;  // Deine Input-Instanz

    void Awake()
    {
        input = new CustomInputs();
        //input.MenuStart.Move.performed += NavigateMenu;
        input.MenuStart.PrimaryAction.performed += SelectMenuItem;
    }

    void OnEnable()
    {
        input.MenuStart.Enable();
        EventSystem.current.SetSelectedGameObject(null);
        SetFirstSelectedButton();
    }

    void OnDisable()
    {
        input.MenuStart.Disable();
    }

    // Dies wird nur zum Debugging genutzt, um zu sehen, welche Inputs ankommen
    //private void NavigateMenu(InputAction.CallbackContext context)
    //{
    //    Debug.Log("Move Input: " + context.ReadValue<Vector2>());
    //}

    private void SelectMenuItem(InputAction.CallbackContext context)
    {
        // Führt die "onClick"-Aktion des derzeit ausgewählten Buttons aus
        EventSystem.current.currentSelectedGameObject?.GetComponent<Button>()?.onClick.Invoke();
    }

    private void SetFirstSelectedButton()
    {
        // Wählt den ersten Button im Menü automatisch aus
        Button firstButton = menuPanel.GetComponentInChildren<Button>();
        if (firstButton != null)
        {
            EventSystem.current.SetSelectedGameObject(firstButton.gameObject);
        }
        else
        {
            Debug.LogError("Keine Buttons im Menü gefunden!");
        }
    }
}
