using UnityEngine;

public class PlantSeeds : MonoBehaviour
{
    public GameObject[] seeds; // Array von Samen-Objekten
    private bool seedsActive = false;
    public bool isWatered = false; // Gibt an, ob die Pflanze gegossen wurde

    private void Awake()
    {
        DeactivateSeeds();
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
        }
    }

    private void ActivateSeeds()
    {
        seedsActive = true;
        foreach (GameObject seed in seeds)
        {
            seed.SetActive(true);
            Rigidbody2D seedRigidbody = seed.GetComponent<Rigidbody2D>();
            if (seedRigidbody != null)
            {
                seedRigidbody.isKinematic = false; // Physik aktivieren
                seedRigidbody.gravityScale = 1f; // Schwerkraft aktivieren
            }
        }
    }

    private void DeactivateSeeds()
    {
        seedsActive = false;
        foreach (GameObject seed in seeds)
        {
            seed.SetActive(false);
            Rigidbody2D seedRigidbody = seed.GetComponent<Rigidbody2D>();
            if (seedRigidbody != null)
            {
                seedRigidbody.isKinematic = true; // Physik deaktivieren
                seedRigidbody.gravityScale = 0f; // Schwerkraft deaktivieren
            }
        }
    }

    public void DropSeeds()
    {
        foreach (GameObject seed in seeds)
        {
            // Füge die Logik hinzu, um die Samen fallen zu lassen, wenn Pitti in die Nähe kommt
            seed.SetActive(true);
            Rigidbody2D seedRigidbody = seed.GetComponent<Rigidbody2D>();
            if (seedRigidbody != null)
            {
                seedRigidbody.isKinematic = false; // Physik aktivieren
                seedRigidbody.gravityScale = 1f; // Schwerkraft aktivieren
            }
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
