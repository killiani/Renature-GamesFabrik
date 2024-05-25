using UnityEngine;

public class BeibootTrigger : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Objects"))
        {
            Rigidbody2D rb = other.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.isKinematic = false;
                rb.simulated = true;
                other.gameObject.layer = LayerMask.NameToLayer("Beiboot");
            }
        }
    }
}
