using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public class HudController : MonoBehaviour
{
    [SerializeField] private GameObject xboxControllerPanel;
    [SerializeField] private GameObject keyboardPanel;

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
}
