using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;
using UnityEngine.UI;

public class HudController : MonoBehaviour
{
    [SerializeField] private GameObject xboxControllerPanel;
    [SerializeField] private GameObject keyboardPanel;

    public Image keyboardBackpack;
    public Image xboxBackpack;

    private string deaktivateColor = "#B7C6DA";
    private string activateColor = "#FFFFFF";

    private void OnEnable()
    {
        InputSystem.onAnyButtonPress.Call(OnAnyInput);
    }

    private void OnDisable()
    {
        InputSystem.onAnyButtonPress.Call(null);
    }

    private void OnAnyInput(InputControl control)
    {
        if (control.device is Gamepad)
        {
            ActivateXboxControllerPanel();
        }
        else if (control.device is Keyboard)
        {
            ActivateKeyboardPanel();
        }
    }

    private void ActivateXboxControllerPanel()
    {
        xboxControllerPanel.SetActive(true);
        keyboardPanel.SetActive(false);
    }

    private void ActivateKeyboardPanel()
    {
        xboxControllerPanel.SetActive(false);
        keyboardPanel.SetActive(true);
    }

    public void ActivateBackpackHud()
    {
        xboxBackpack.color = ConvertHexToColor(activateColor);
        keyboardBackpack.color = ConvertHexToColor(activateColor);
    }

    public void DeaktivateBackpackHud()
    {
        xboxBackpack.color = ConvertHexToColor(deaktivateColor);
        keyboardBackpack.color = ConvertHexToColor(deaktivateColor);
    }

    private Color ConvertHexToColor(string hexColor)
    {
        Color newColor;
        if (UnityEngine.ColorUtility.TryParseHtmlString(hexColor, out newColor))
        {
            return newColor;
        }
        else
        {
            Debug.LogError("Ungültiger Hexwert für die Farbe!");
            return Color.white;
        }
    }
}
