using UnityEngine;

public class NightZone : MonoBehaviour
{
    public Animator pittiAnimator; // Referenz zum Animator von Pitti
    private PlayerMovement playerMovement; // Referenz zum PlayerMovement-Skript
    private RotateObject skyDiscRotateObject; // Referenz zum RotateObject-Skript
    private bool hasTriggeredNightEvent = false; // Variable zum Überprüfen, ob das Ereignis bereits ausgelöst wurde

    void Start()
    {
        if (pittiAnimator == null)
        {
            Debug.LogError("Animator not assigned in the inspector.");
        }

        // PlayerMovement-Skript automatisch finden
        playerMovement = FindObjectOfType<PlayerMovement>();
        if (playerMovement == null)
        {
            Debug.LogError("PlayerMovement script not found in the scene.");
        }

        // RotateObject-Skript auf der Sky-Scheibe finden
        GameObject skyDisc = GameObject.FindWithTag("SkyDisc");
        if (skyDisc != null)
        {
            skyDiscRotateObject = skyDisc.GetComponent<RotateObject>();
            if (skyDiscRotateObject == null)
            {
                Debug.LogError("RotateObject script not found on SkyDisc.");
            }
        }
        else
        {
            Debug.LogError("SkyDisc GameObject not found.");
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("NightMarker") && !hasTriggeredNightEvent)
        {
            TriggerNightEvent();
        }
    }

    void TriggerNightEvent()
    {
        // Setze die Nacht-Animation von Pitti
        pittiAnimator.SetTrigger("IsNight");

        // Zeige die Nacht-Denkblase an, falls PlayerMovement gefunden wurde
        if (playerMovement != null)
        {
            playerMovement.ShowNightThinkingBubble();
        }

        // Stoppe die Rotation
        if (skyDiscRotateObject != null)
        {
            skyDiscRotateObject.StopRotation();
        }

        // Setze die Variable auf true, um zu verhindern, dass das Ereignis erneut ausgelöst wird
        hasTriggeredNightEvent = true;
    }
}
