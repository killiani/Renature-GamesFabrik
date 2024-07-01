using UnityEngine;

public class SeedTrigger : MonoBehaviour
{
    public float detectionRange = 1.0f; // Die Reichweite, in der Pitti die Samen fallen lassen kann
    private Transform playerTransform;
    private PlantSeeds plant;

    private void Start()
    {
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        plant = GetComponentInParent<PlantSeeds>();
        Debug.Log("SeedTrigger runs");
    }

    private void FixedUpdate()
    {
        if (playerTransform != null && plant != null)
        {
            // Überprüfe die X-Distanz zwischen Pitti und der Pflanze
            if (Mathf.Abs(playerTransform.position.x - transform.position.x) <= detectionRange)
            {
                plant.DropSeeds();
            }
        }
    }
}
