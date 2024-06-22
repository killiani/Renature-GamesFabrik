using UnityEngine;

public class NightZone : MonoBehaviour
{
    public Animator pittiAnimator; // Referenz zum Animator von Pitti
    private PlayerMovement playerMovement;

    void Start()
    {
        if (pittiAnimator == null)
        {
            Debug.LogError("Animator not assigned in the inspector.");
        }
        playerMovement = FindObjectOfType<PlayerMovement>();
        if (playerMovement == null)
        {
            Debug.LogError("PlayerMovement script not assigned in the inspector.");
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("NightMarker"))
        {
            TriggerNightEvent();
        }
    }

    void TriggerNightEvent()
    {
        pittiAnimator.SetTrigger("IsNight");
        // Zeige die Nacht-Denkblase an
        if (playerMovement != null)
        {
            playerMovement.ShowNightThinkingBubble();
        }
    }
}
