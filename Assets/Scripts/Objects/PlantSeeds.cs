using System.Collections;
using UnityEngine;

public class PlantSeeds : MonoBehaviour
{
    public GameObject[] seeds; // Array von Samen-Objekten
    private Vector3[] originalPositions;
    private bool seedsActive = false;
    public bool isWatered = false; // Gibt an, ob die Pflanze gegossen wurde
    private int targetSeedLayer = -1;
    private bool seedsDropped = false;

    private void Awake()
    {
        // Speichere die ursprünglichen Positionen der Samen - damit sie immmer wieder von der Pflanze fallen
        originalPositions = new Vector3[seeds.Length];
        for (int i = 0; i < seeds.Length; i++)
        {
            originalPositions[i] = seeds[i].transform.localPosition;
        }
        DeactivateSeeds();
    }

    public void WaterPlant()
    {
        isWatered = true;
    }

    private void ActivateSeeds()
    {
        DeactivateSeeds(); // bereits auf dem Boden liegende Samen werden geloescht
        seedsActive = true;
        foreach (GameObject seed in seeds)
        {
            seed.SetActive(true);
        }
    }

    public void DeactivateSeeds()
    {
        seedsActive = false;
        seedsDropped = false;
        for (int i = 0; i < seeds.Length; i++)
        {
            GameObject seed = seeds[i];
            seed.SetActive(false);
            seed.transform.localPosition = originalPositions[i]; // Setze die ursprüngliche Position zurück
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
        if (isWatered && seedsActive && !seedsDropped)
        {
            StartCoroutine(DropSeedsAfterDelay());
        }
    }
    public IEnumerator DropSeedsAfterDelay()
    {
        isWatered = false;
        seedsDropped = true;
        yield return new WaitForSeconds(3f);

        foreach (GameObject seed in seeds)
        {
            
            SpriteRenderer seedSpriteRenderer = seed.GetComponent<SpriteRenderer>();
            if (seedSpriteRenderer != null)
            {
                seedSpriteRenderer.sortingOrder = targetSeedLayer;
            }

            Rigidbody2D seedRigidbody = seed.GetComponent<Rigidbody2D>();
            if (seedRigidbody != null)
            {
                seedRigidbody.isKinematic = false; // Physik aktivieren
                seedRigidbody.gravityScale = 1f; // Schwerkraft aktivieren
                seedRigidbody.velocity = Vector2.zero; 
                Debug.Log("Samen fallen: " + seed.name);
            }
        }
    }

    public void GrowSeedsOvernight()
    {
        if (isWatered)
        {
            ActivateSeeds(); // Samen aktivieren, wenn die Pflanze gegossen wurde
        }
    }
}
