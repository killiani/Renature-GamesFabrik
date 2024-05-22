using UnityEngine;

public class BeibootTrigger : MonoBehaviour
{
    public bool characterInZone;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            characterInZone = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            characterInZone = false;
        }
    }
}
