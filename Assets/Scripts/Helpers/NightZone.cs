using UnityEngine;

public class NightZone : MonoBehaviour
{
    public Animator pittiAnimator; // Referenz zum Animator von Pitti

    void Start()
    {
        if (pittiAnimator == null)
        {
            Debug.LogError("Animator not assigned in the inspector.");
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
    }
}
