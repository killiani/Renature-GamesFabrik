using UnityEngine;

public class PlantingZoneTrigger : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D other)
    {
        Block block = other.GetComponent<Block>();
        if (block != null)
        {
            // Hier könnte eine andere Logik stehen
            // block.DeactivateGlow(); -> entfernen oder anpassen
            //Debug.Log("Pitti entered a block.");
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        Block block = other.GetComponent<Block>();
        if (block != null)
        {
            // Hier könnte eine andere Logik stehen
            // block.DeactivateGlow(); -> entfernen oder anpassen
            //Debug.Log("Pitti left a block.");
        }
    }
}
