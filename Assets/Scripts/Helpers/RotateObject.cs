using UnityEngine;

public class RotateObject : MonoBehaviour
{
    public float rotationSpeed = 0.31f;
    private float targetRotationSpeed = 18f;
    public bool rotate = false;

    void Update()
    {
        float speed = rotate ? targetRotationSpeed : rotationSpeed;
        transform.Rotate(0, 0, speed * Time.deltaTime);
    }

    public void StartRotationToNightZone()
    {
        rotate = true;
    }

    public void StartRotationToDayZone()
    {
        rotate = true;
    }

    public void StopRotation()
    {
        rotate = false;
        rotationSpeed = 0.31f; // Zurück auf normale Geschwindigkeit
    }

    public void StartRotation()
    {
        rotationSpeed = 0.31f;
    }
}
