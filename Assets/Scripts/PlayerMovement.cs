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
    [SerializeField] private GameObject plantPrefab; // Prefab für die Erde wo die plants hinkommen
    [SerializeField] private GameObject mangrovePrefab; // Prefab für Mangrove
    [SerializeField] private GameObject farnPrefab; // Prefab für Farn
    [SerializeField] private Transform frontHandPosition;
    [SerializeField] private GameObject seedPrefab; // Generisches Samen-Prefab
    [SerializeField] private BeibootTrigger beibootTrigger; // Referenz zum Beiboot

    // Diese Referenz wird im Start-Methodenblock automatisch gesetzt
    private PickupScript pickupScript;
    private Backpack backpack;
    private GameObject nearestObject;
    private GameObject seedInHand; // Referenz auf den Samen in Pittis Hand


    void Awake()
    {
        input = new CustomInputs(); // Initialisiere das neue Input System

        // Binde die Bewegungsaktion an die Move-Methode
        input.Player.Move.performed += OnMove;
        input.Player.Move.canceled += OnMoveCanceled;

        // Binde die Primäraktion an die HandlePickupDrop-Methode
        input.Player.PrimaryAction.performed += ctx => HandlePickupDrop();

        input.Player.PlantAction.performed += ctx => HandlePlanting();
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

        pickupScript = GetComponent<PickupScript>();

        if (pickupScript == null)
        {
            Debug.LogError("Kein PickupScript-Komponente im GameObject gefunden.");
        }

        if (beibootTrigger == null)
        {
            Debug.LogError("Kein BeibootTrigger-Komponente im GameObject gefunden.");
        }

        backpack = GetComponent<Backpack>();

        if (backpack == null)
        {
            Debug.LogError("Kein Backpack-Komponente im GameObject gefunden. Stelle sicher, dass das Backpack-Skript hinzugefügt ist.");
            // Optional: Automatisch hinzufügen, falls es fehlt
            backpack = gameObject.AddComponent<Backpack>();
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



    private void HandlePlanting()
    {
        if (backpack.GetSeedCount() > 0)
        {
            animator.SetTrigger("HandleGoPlant");

            // Zeige den Samen in Pittis Hand während der Animation
            //seedInHand = Instantiate(seedPrefab, frontHandPosition.position, Quaternion.identity);
            //seedInHand.transform.SetParent(frontHandPosition);
            //seedInHand.SetActive(true);
        }
        else
        {
            Debug.Log("No seeds available to plant.");
        }
    }


    public void OnPlantingAnimationSeed()
    {
        if (seedInHand != null)
        {
            Destroy(seedInHand); // Entferne den Samen aus Pittis Hand
        }
    }

    public void OnPlantingAnimation()
    {

        Seed seedToPlant = backpack.GetAndRemoveLastSeed();
        if (seedToPlant != null)
        {
            Plant newPlant = new Plant((Plant.PlantType)seedToPlant.Type, seedToPlant.GrowthTime);

            float offsetY = 0.5f; // versatz nach unten
            float offsetX = 0.0f; // versatz L oder R

            if (isFacingRight)
            {
                offsetX = 0.5f;
            } else
            {
                offsetX = -0.5f;
            }

            Vector3 plantPosition = new Vector3(transform.position.x + offsetX, groundCheck.position.y + offsetY, transform.position.z); // Position von Pittis Füßen
            GameObject plantInstance = Instantiate(plantPrefab, plantPosition, Quaternion.identity);

            // Starte die Wachstumsroutine
            StartCoroutine(GrowPlant(plantInstance, newPlant.Type, newPlant.GrowthTime));

            //Debug.Log($"Planted a {newPlant.Type} seed with growth time of {newPlant.GrowthTime} seconds.");
            //Debug.Log($"Planted a {newPlant.Type} seed with growth time of {newPlant.GrowthTime} seconds.");
            //Debug.Log($"Plant position: {plantPosition}");
            //Debug.Log($"Pitti position: {transform.position}");
            //Debug.Log($"GroundCheck position: {groundCheck.position}");
        }
    }


    private void HandlePickupDrop()
    {
        animator.SetTrigger("HandleGoDown");

        if (switchMovement == false) // __________________________ AUFHEBEN
        {
            // Setze den Zustand auf Aufheben und überprüfe, ob sich ein Objekt in der Nähe befindet
            nearestObject = GetNearestObject();
            Debug.Log("nearest: ", nearestObject);

            if (nearestObject != null)
            {
                if (nearestObject.layer == LayerMask.NameToLayer("Seeds"))
                {
                    Debug.Log("Try to grab Seed");
                } 
                else if (nearestObject.layer == LayerMask.NameToLayer("Objects"))
                {
                    Debug.Log("Try to grab Object");
                }
                isPickingUp = true; // Animation auslösen
            }
        }
        else // __________________________________________ ABLEGEN
        {
            if (pickupScript.carriedObject != null)
            {
                Rigidbody2D rb = pickupScript.carriedObject.GetComponent<Rigidbody2D>();
                Collider2D col = pickupScript.carriedObject.GetComponent<Collider2D>();

                Debug.Log("Drop OK");

                if (rb != null)
                {
                    rb.isKinematic = false; // Physik wieder aktivieren
                    rb.simulated = true; // Simulation aktivieren
                }
                if (col != null)
                {
                    col.enabled = true; // Collider aktivieren
                }

                pickupScript.DropObject();

                switchMovement = false;
                animator.SetBool("HasObject", false);
            }
        }
    }


    // Diese Methode wird vom Animationsevent in der Mitte aufgerufen
    public void OnPickupAnimationEnd()
    {
        Debug.Log("Zustand: " + switchMovement);
        if (isPickingUp && !switchMovement && nearestObject != null)
        {
            if (nearestObject.layer == LayerMask.NameToLayer("Objects"))
            {
                pickupScript.carriedObject = nearestObject; // Setze das zu tragende Objekt
                pickupScript.PickupObject(pickupScript.carriedObject);

                switchMovement = true;
                animator.SetBool("HasObject", true);
                Debug.Log("Object Pickup OK");
            }

            else if (nearestObject.layer == LayerMask.NameToLayer("Seeds"))
            {
                if (backpack != null)
                {
                    //backpack.AddSeed(nearestObject);
                    Seed randomSeed = Seed.GenerateRandomSeed();
                    backpack.AddSeed(randomSeed);
                    Debug.Log("Seed Pickup OK");
                    nearestObject.SetActive(false); // Samen deaktivieren, nachdem er aufgesamme
                }
                else
                {
                    Debug.Log("Backpack is null");
                }
                nearestObject = null;
            }
        }
        // Setze den Zustand nach dem Aufheben oder Ablegen zurück
        isPickingUp = false;
    }

    // Methode zur Überprüfung, ob sich ein aufhebbares Objekt in der Nähe befindet

    private GameObject GetNearestObject()
    {
        // Erstelle einen Kreis-Collider, um nach Objekten und Samen zu suchen
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, 1f, LayerMask.GetMask("Objects", "Seeds"));
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

    private IEnumerator GrowPlant(GameObject plantInstance, Plant.PlantType plantType, float growTime)
    {
        yield return new WaitForSeconds(growTime); // Warte für die Dauer der Wachstumszeit

        // Bestimme das richtige Prefab basierend auf dem Pflanzentyp
        GameObject newPlantPrefab = null;
        switch (plantType)
        {
            case Plant.PlantType.Mangrove:
                newPlantPrefab = mangrovePrefab;
                break;
            case Plant.PlantType.Farn:
                newPlantPrefab = farnPrefab;
                break;
        }

        if (newPlantPrefab != null)
        {
            // Speichere die Position der aktuellen Pflanze
            Vector3 plantPosition = plantInstance.transform.position;

            // Zerstöre die alte Pflanze
            Destroy(plantInstance);

            // Erstelle die neue Pflanze an derselben Position
            Instantiate(newPlantPrefab, plantPosition, Quaternion.identity);
        }
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
