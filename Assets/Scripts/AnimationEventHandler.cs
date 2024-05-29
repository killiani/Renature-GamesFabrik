using UnityEngine;

public class AnimationEventHandler : MonoBehaviour
{
    private PlayerMovement playerMovement;

    void Start()
    {
        // Suche das Elternobjekt und hole das PlayerMovement-Skript
        playerMovement = GetComponentInParent<PlayerMovement>();

        if (playerMovement == null)
        {
            Debug.LogError("Kein PlayerMovement-Skript im Elternobjekt gefunden.");
        }
    }

    // Diese Methoden werden vom Animationsevent aufgerufen
    public void OnPickupAnimationEnd()
    {
        if (playerMovement != null)
        {
            playerMovement.OnPickupAnimationEnd();
        }
    }

    public void OnPlantingAnimation()
    {
        if (playerMovement != null)
        {
            playerMovement.OnPlantingAnimation();
        }
    }

    public void OnPlantingAnimationSeed()
    {
        if (playerMovement != null)
        {
            playerMovement.OnPlantingAnimationSeed();
        }
    }
}
