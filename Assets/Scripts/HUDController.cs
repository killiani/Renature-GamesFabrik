using UnityEngine;
using UnityEngine.UI;

public class HUDController : MonoBehaviour
{
    public Image EButton;
    public Image[] keyImages;

    private CustomInputs input;

    void Awake()
    {
        input = new CustomInputs();
    }

    void OnEnable()
    {
        input.Enable();

        input.HUD.HideButton.performed += ctx => ShowKeys(); //  anzeigen bei gedrückt halten
        input.HUD.HideButton.canceled += ctx => HideKeys(); // verbergen

        // Binde andere Tastenaktionen hier
        //inputActions.UI.Key1.performed += ctx => OnKey1Pressed();
        //input.Player.PlantAction.performed += ctx => OnKey2Pressed();
        //inputActions.UI.Key3.performed += ctx => OnKey3Pressed();
        //inputActions.UI.Key4.performed += ctx => OnKey4Pressed();
        //inputActions.UI.Key5.performed += ctx => OnKey5Pressed();
    }

    void OnDisable()
    {
        input.Disable();
    }

    void Start()
    {
        // Stelle sicher, dass die Key-Bilder am Anfang ausgeblendet sind
        foreach (Image img in keyImages)
        {
            img.gameObject.SetActive(false);
        }
    }

    void ShowKeys()
    {
        foreach (Image img in keyImages)
        {
            img.gameObject.SetActive(true);
        }
    }

    void HideKeys()
    {
        foreach (Image img in keyImages)
        {
            img.gameObject.SetActive(false);
        }
    }

    //void OnKey2Pressed()
    //{
    //    Debug.Log("Key2 pressed");
    //}
}
