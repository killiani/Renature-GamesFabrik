using UnityEditor;
using UnityEngine;

// Script für Müllobjekte
public class Object : MonoBehaviour
{
    private Rigidbody2D rb;
    private Collider2D col;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();

        // Stellen Sie sicher, dass das Objekt nicht durch Kollision bewegt werden kann
        rb.bodyType = RigidbodyType2D.Static;
    }

    // Aufheben
    public void Aufheben()
    {
        rb.bodyType = RigidbodyType2D.Dynamic;
        gameObject.layer = LayerMask.NameToLayer("Weare-Objects");
    }

    // Ablegen
    public void Ablegen()
    {
        rb.bodyType = RigidbodyType2D.Static;
        gameObject.layer = LayerMask.NameToLayer("Objects");
    }
}
