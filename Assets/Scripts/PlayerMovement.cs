using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private float horizontal;
    private float speed = 1.5f;
    private bool isFacingRight = true;

    private Animator animator;
    private Collider2D platformCollider;

    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundLayer;

    void Start()
    {
        animator = GetComponentInChildren<Animator>(); // da Pitti in ParentPitti liegt

        if (animator == null)
        {
            Debug.LogError("Kein Animator-Komponente im GameObject 'ParentPitti' oder seinen Kindern gefunden.");
        }
    }

    void Update()
    {
        horizontal = Input.GetAxisRaw("Horizontal");
        bool isMoving = Mathf.Abs(horizontal) > 0;
        animator.SetBool("IsRunning", isMoving);

        //Debug.Log($"Horizontal Input: {horizontal}, Is Running: {animator.GetBool("IsRunning")}");

        Flip();

    }

    private void FixedUpdate()
    {
        rb.velocity = new Vector2(horizontal * speed, rb.velocity.y);
    }

    private bool IsGrounded()
    {
        return Physics2D.OverlapCircle(groundCheck.position, 0.2f, groundLayer);
    }

    private void Flip()
    {
        if (isFacingRight && horizontal < 0f || !isFacingRight && horizontal > 0f)
        {
            isFacingRight = !isFacingRight;
            Vector3 localScale = transform.localScale;
            localScale.x *= -1f;
            transform.localScale = localScale;
        }
    }
}
