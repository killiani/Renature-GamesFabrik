using UnityEngine;

public class HillOfDirt : MonoBehaviour
{
    public bool isWatered = false;
    private string wet = "#97A6BE";
    //private string dry = "#FFFFFF";
    public Plant.PlantType plantType;
    public Vector3 plantPosition;

    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void WaterHill()
    {
        isWatered = true;
        spriteRenderer.color = ConvertHexToColor(wet);
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
