using UnityEngine;

public class HouseTrigger : MonoBehaviour
{
    private bool playerInZone = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInZone = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInZone = false;
        }
    }

    public bool IsPlayerInZone()
    {
        return playerInZone;
    }
}
