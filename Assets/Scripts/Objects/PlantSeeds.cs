using UnityEngine;

public class PlantSeeds : MonoBehaviour
{
    public GameObject[] seeds; // Array von Samen-Objekten
    private bool seedsActive = false;
    public bool isWatered = false; // Gibt an, ob die Pflanze gegossen wurde

    private void Awake()
    {
        DeactivateSeeds();
        Debug.Log("PlantSeeds runs");
    }

    public void WaterPlant()
    {
        if (!isWatered)
        {
            isWatered = true;
            if (!seedsActive)
            {
                ActivateSeeds();
            }
            Debug.Log($"{gameObject.name} wurde gegossen!"); // Debug-Log hinzugefügt
        }
        else
        {
            Debug.Log($"{gameObject.name} ist bereits gegossen!"); // Debug-Log hinzugefügt
        }
    }

    private void ActivateSeeds()
    {
        seedsActive = true;
        foreach (GameObject seed in seeds)
        {
            seed.SetActive(true);
        }
    }

    private void DeactivateSeeds()
    {
        seedsActive = false;
        foreach (GameObject seed in seeds)
        {
            seed.SetActive(false);
        }
    }

    public void DropSeeds()
    {
        foreach (GameObject seed in seeds)
        {
            // Füge die Logik hinzu, um die Samen fallen zu lassen
            seed.SetActive(false);
        }
        seedsActive = false;
    }

    public void GrowSeedsOvernight()
    {
        if (seedsActive)
        {
            DeactivateSeeds(); // Deaktiviere die aktuellen Samen
        }
        ActivateSeeds(); // Aktiviere neue Samen
    }
}
