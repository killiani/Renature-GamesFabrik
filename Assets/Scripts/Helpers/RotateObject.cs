using UnityEngine;

public class RotateObject : MonoBehaviour
{
    // Geschwindigkeit der Rotation in Grad pro Sekunde
    public float rotationSpeed = 0.31f;

    void Update()
    {
        // Rotieren um die Z-Achse (für eine 2D-Ansicht, passe die Achse an, wenn nötig)
        transform.Rotate(0, 0, rotationSpeed * Time.deltaTime);
    }
}
