using UnityEngine;

public class NightZone : MonoBehaviour
{
    public Animator pittiAnimator; // Referenz zum Animator von Pitti
    private PlayerMovement playerMovement; // Referenz zum PlayerMovement-Skript
    private RotateObject skyDiscRotateObject; // Referenz zum RotateObject-Skript
    public bool hasTriggeredNightEvent = false; // Variable zum Überprüfen, ob das Ereignis bereits ausgelöst wurde

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
        if(other.CompareTag("NightReadyMarker"))
        {
            playerMovement.EnableNightAction();
            Debug.Log("Night Ready!!!!");
        }

        if (other.CompareTag("NightMarker") && !hasTriggeredNightEvent)
        {
            TriggerNightEvent();
        }

        else if (other.CompareTag("DayMarker"))
        {
            // Stoppe die Rotation, wenn der DayMarker die NightZone erreicht
            if (skyDiscRotateObject != null)
            {
                skyDiscRotateObject.StopRotation();
            }
        }
    }

    void TriggerNightEvent()
    {
        pittiAnimator.SetTrigger("IsNight"); // Pitti schaut nach oben

        if (playerMovement != null)
        {
            playerMovement.ShowNightThinkingBubble();
        }

        if (skyDiscRotateObject != null)
        {
            skyDiscRotateObject.StopRotation();
        }

        // Setze die Variable auf true, um zu verhindern, dass das Ereignis erneut ausgelöst wird
        hasTriggeredNightEvent = true;
    }

    public void TriggerDayEvent()
    {
        if (skyDiscRotateObject != null)
        {
            skyDiscRotateObject.StartRotationToDayZone();
        }
    }
}
