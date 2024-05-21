using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem; // Importiere das neue Input System

public class PlayerMovement : MonoBehaviour
{
    private float horizontal;
    private float speed = 1.5f;
    private bool isFacingRight = true;
    private bool switchMovement = false; // Gibt an, ob der Charakter ein Objekt trägt
    private bool isPickingUp = false; // Zustand des Aufhebens/Ablegens

    private Animator animator;
    private Collider2D platformCollider;
    private CustomInputs input; // Referenz zum neuen Input System

    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private AudioSource walkSoundSource; // AudioSource für den Walksound
    [SerializeField] private AudioClip walkSound; // Walksound AudioClip

    // Diese Referenz wird im Start-Methodenblock automatisch gesetzt
    private PickupScript pickupScript;

    void Awake()
    {
        input = new CustomInputs(); // Initialisiere das neue Input System

        // Binde die Bewegungsaktion an die Move-Methode
        input.Player.Move.performed += OnMove;
        input.Player.Move.canceled += OnMoveCanceled;

        // Binde die Primäraktion an die HandlePickupDrop-Methode
        input.Player.PrimaryAction.performed += ctx => HandlePickupDrop();
    }

    void OnEnable()
    {
        input.Player.Enable(); // Aktiviere die Eingaben
    }

    void OnDisable()
    {
        input.Player.Disable(); // Deaktiviere die Eingaben
    }

    void Start()
    {
        animator = GetComponentInChildren<Animator>(); // da Pitti in ParentPitti liegt

        if (animator == null)
        {
            Debug.LogError("Kein Animator-Komponente im GameObject 'ParentPitti' oder seinen Kindern gefunden.");
        }

        // Automatische Zuweisung des PickupScript
        pickupScript = GetComponent<PickupScript>();

        if (pickupScript == null)
        {
            Debug.LogError("Kein PickupScript-Komponente im GameObject gefunden.");
        }
    }

    void Update()
    {
        Moving();
    }

    private void OnMove(InputAction.CallbackContext context)
    {
        Vector2 inputVector = context.ReadValue<Vector2>();
        horizontal = inputVector.x;
    }

    private void OnMoveCanceled(InputAction.CallbackContext context)
    {
        horizontal = 0f;
    }

    private void HandlePickupDrop()
    {
        animator.SetTrigger("HandleGoDown");

        if (!switchMovement)
        {
            // Setze den Zustand auf Aufheben und überprüfe, ob sich ein Objekt in der Nähe befindet
            GameObject nearestObject = GetNearestObject();
            if (nearestObject != null)
            {
                isPickingUp = true;
                pickupScript.carriedObject = nearestObject; // Setze das zu tragende Objekt
                Debug.LogError("Try to Grab");
            }
        }
        else
        {
            // Setze den Zustand auf Ablegen
            isPickingUp = false;

            // Objekt ablegen
            if (pickupScript.carriedObject != null)
            {
                Debug.LogError("Drop OK");
                pickupScript.DropObject();

                switchMovement = false;
                animator.SetBool("HasObject", switchMovement);
            }
        }
    }

    // Methode zur Überprüfung, ob sich ein aufhebbares Objekt in der Nähe befindet
    private GameObject GetNearestObject()
    {
        // Erstelle einen Kreis-Collider, um nach Objekten zu suchen
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, 1f, LayerMask.GetMask("Objects"));
        GameObject nearestObject = null;
        float nearestDistance = float.MaxValue;

        foreach (Collider2D collider in colliders)
        {
            if (collider.gameObject != gameObject)
            {
                float distance = Vector2.Distance(transform.position, collider.transform.position);
                if (distance < nearestDistance)
                {
                    nearestDistance = distance;
                    nearestObject = collider.gameObject;
                }
            }
        }
        return nearestObject;
    }

    // Diese Methode wird vom Animationsevent aufgerufen
    public void OnPickupAnimationEnd()
    {
        Debug.Log("Zustand: " + switchMovement);
        if (isPickingUp && !switchMovement && pickupScript.carriedObject != null)
        {
            Debug.LogError("Pickup OK");
            pickupScript.PickupObject(pickupScript.carriedObject);

            switchMovement = true;
            animator.SetBool("HasObject", switchMovement);
        }
        // Setze den Zustand nach dem Aufheben oder Ablegen zurück
        isPickingUp = false;
    }

    private void FixedUpdate()
    {
        rb.velocity = new Vector2(horizontal * speed, rb.velocity.y);
    }

    private void Moving()
    {
        bool isMoving = Mathf.Abs(horizontal) > 0;
        animator.SetBool("IsRunning", isMoving);

        WalkSound(isMoving);
        Flip();
    }

    private void WalkSound(bool isMoving)
    {
        // Spiele Walksound ab, wenn der Charakter sich bewegt
        if (isMoving)
        {
            if (!walkSoundSource.isPlaying)
            {
                walkSoundSource.clip = walkSound;
                walkSoundSource.loop = true;
                walkSoundSource.Play();
            }
        }
        else
        {
            if (walkSoundSource.isPlaying)
            {
                walkSoundSource.Stop();
                walkSoundSource.clip = walkSound; // Setze den Clip zurück, um Verzögerungen zu minimieren
            }
        }
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
