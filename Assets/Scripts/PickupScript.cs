using UnityEngine;

public class PickupScript : MonoBehaviour
{
    public Transform frontHandPosition; // Referenz zur Front-Hand-Position des Charakters
    public Transform backHandPosition; // Referenz zur Back-Hand-Position des Charakters
    [HideInInspector] public GameObject carriedObject; // Das aktuell getragene Objekt

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Objects") && carriedObject == null)
        {
            Debug.Log("Object detected: " + other.gameObject.name);
            carriedObject = other.gameObject;
        }
    }

    public void PickupObject(GameObject obj)
    {
        // Mache das Rigidbody kinematisch, um Physik zu deaktivieren
        Rigidbody2D rb = obj.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.isKinematic = true; // Deaktiviere Physik
            rb.simulated = false; // Deaktiviere Simulation
        }

        // Setze das Objekt als Kind der Hand-Position
        obj.transform.SetParent(frontHandPosition);
        obj.transform.localPosition = Vector3.zero;
        obj.transform.localRotation = Quaternion.identity;
    }

    public void DropObject()
    {
        if (carriedObject != null)
        {
            // Reaktiviere Physik des Objekts
            Rigidbody2D rb = carriedObject.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.isKinematic = false; // Reaktiviere Physik
                rb.simulated = true; // Reaktiviere Simulation
            }

            // Trenne das Objekt von der Hand
            carriedObject.transform.SetParent(null);
            carriedObject = null;
        }
    }
}
