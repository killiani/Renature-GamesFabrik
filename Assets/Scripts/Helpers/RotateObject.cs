using UnityEngine;

public class RotateObject : MonoBehaviour
{
    // Geschwindigkeit der Rotation in Grad pro Sekunde
    public float rotationSpeed = 0.31f;
    public bool rotateToNightZone = false;
    private float targetRotationSpeed = 15f;

    void Update()
    {
        float speed = rotateToNightZone ? targetRotationSpeed : rotationSpeed;
        transform.Rotate(0, 0, speed * Time.deltaTime);
    }

    public void StartRotationToNightZone()
    {
        rotateToNightZone = true;
    }

    public void StopRotation()
    {
        rotateToNightZone = false;
        rotationSpeed = 0.0f;
    }

    public void StartRotation()
    {
        rotationSpeed = 0.31f;
    }
}

