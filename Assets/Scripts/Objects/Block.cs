using System.Collections.Generic;
using UnityEngine;
using static Unity.Collections.AllocatorManager;

public class Block : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    public Color objectGlowColor = Color.red;
    private Color originalColor;
    private bool hasObjectAbove = false;
    public float checkHeight = 2f; // Height to check above
    public float checkWidth = 1f; // Width of the BoxCast

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            originalColor = spriteRenderer.color;
        }

        // Initial check for objects above
        CheckForObjectsAbove();
        UpdateColor();
    }

    private void CheckForObjectsAbove()
    {
        Collider2D[] hits = Physics2D.OverlapBoxAll(
            new Vector2(transform.position.x, transform.position.y + checkHeight / 2),
            new Vector2(checkWidth, checkHeight),
            0f,
            LayerMask.GetMask("Objects"));

        hasObjectAbove = (hits.Length > 0);
    }

    private void UpdateColor()
    {
        if (spriteRenderer != null)
        {
            if (hasObjectAbove)
            {
                spriteRenderer.color = objectGlowColor;
            }
            else
            {
                spriteRenderer.color = originalColor;
            }
        }
    }

    public bool CheckPosition()
    {
        return hasObjectAbove;
    }

    public void ShowBlock()
    {
        gameObject.SetActive(true);
    }

    public void HideBlock()
    {
        gameObject.SetActive(false);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (hasObjectAbove)
            {
                //Debug.Log("Pitti is on a red block.");
            }
            else
            {
                //Debug.Log("Pitti is on a non-red block.");
            }
        }

        if (other.gameObject.layer == LayerMask.NameToLayer("Objects"))
        {
            hasObjectAbove = true;
            UpdateColor();
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            //Debug.Log("Pitti has left the block.");
        }

        if (other.gameObject.layer == LayerMask.NameToLayer("Objects"))
        {
            hasObjectAbove = false;
            UpdateColor();
        }
    }

    void Update()
    {
        CheckForObjectsAbove();
        UpdateColor();
    }
}
