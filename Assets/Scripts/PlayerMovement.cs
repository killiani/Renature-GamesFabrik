using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem; // Importiere das neue Input System

public class PlayerMovement : MonoBehaviour
{
    private float horizontal;
    private float speed = 1.5f; // walk
    private float speedFaster = 2.3f; // running
    private bool isRunning = false;
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
    [SerializeField] private AudioSource runningSoundSource;
    [SerializeField] private AudioClip runningSound;
    [SerializeField] private GameObject plantPrefab; // Prefab für die Erde wo die plants hinkommen
    [SerializeField] private GameObject mangrovePrefab; // Prefab für Mangrove
    [SerializeField] private GameObject farnPrefab;
    [SerializeField] private GameObject alocasiaPrefab;
    [SerializeField] private GameObject crotonPrefab;
    [SerializeField] private GameObject teaktreePrefab;
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
        input.Player.RunningFaster.performed += ctx => OnRun(ctx);
        input.Player.RunningFaster.canceled += ctx => OnRunCanceled(ctx);
        input.Player.PrimaryAction.performed += HandlePrimaryAction;
        input.Player.WateringAction.performed += HandleWateringAction;
    }

    public void DisableMovement() // BackpackController steuert dies um die Tasten der Auwahl zuzuordnen
    {
        input.Player.Move.performed -= OnMove;
        input.Player.Move.canceled -= OnMoveCanceled;
        input.Player.PrimaryAction.performed -= HandlePrimaryAction;
        input.Player.WateringAction.performed -= HandleWateringAction;
    }
    public void EnableMovement()
    {
        StartCoroutine(EnableMovementAfterDelay());
    }

    private IEnumerator EnableMovementAfterDelay()
    {
        yield return new WaitForSeconds(0.1f);  // Verzögerung von 100 Millisekunden
        input.Player.Move.performed += OnMove;
        input.Player.Move.canceled += OnMoveCanceled;
        input.Player.PrimaryAction.performed += HandlePrimaryAction;
        input.Player.WateringAction.performed += HandleWateringAction;
        Debug.Log("Movement Enabled");
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
        if (input.Player.enabled) // Überprüfen Sie, ob die Player-Eingaben aktiviert sind
        {
            Vector2 inputVector = context.ReadValue<Vector2>();
            horizontal = inputVector.x;
        }
    }

    private void OnMoveCanceled(InputAction.CallbackContext context)
    {
        horizontal = 0f;
    }

    private void OnRun(InputAction.CallbackContext context)
    {
        isRunning = true;
    }

    private void OnRunCanceled(InputAction.CallbackContext context)
    {
        isRunning = false;
    }

    public void OnPlantingAnimationSeedStart()
    {
        // Samen in die Hand positionieren
        seedInHand = Instantiate(seedPrefab, frontHandPosition.position, Quaternion.identity, frontHandPosition);
        seedInHand.transform.localScale *= 3; // Vergrößern 

        // Sicherstellen, dass der Samen kein Rigidbody hat oder kinematisch ist
        Rigidbody2D seedRigidbody = seedInHand.GetComponent<Rigidbody2D>();
        if (seedRigidbody != null)
        {
            seedRigidbody.isKinematic = true;
        }
    }

    public void OnPlantingAnimationSeed()
    {
        if (seedInHand != null)
        {
            Destroy(seedInHand); // Entferne den Samen aus Pittis Hand
        }
    }


    /* Einpflanz Animation mit Samen aus Rucksack
     * 
     * - Neue Variable zum Speichern des aktuellen Samenindex
     * - Neue Methode zum Setzen des Samenindex, asu BackpackController
     * 
     */

    private float GetGroundAngle(Vector3 position)
    {
        // Führen Sie einen Raycast nach unten durch, um den Boden zu treffen
        RaycastHit2D hit = Physics2D.Raycast(position, Vector2.down, 1f, groundLayer);
        if (hit.collider != null)
        {
            // Berechnen Sie den Winkel des Bodens an der Trefferstelle
            Vector2 normal = hit.normal;
            float angle = Mathf.Atan2(normal.y, normal.x) * Mathf.Rad2Deg;
            return angle - 90f; // Anpassen, damit 0 Grad horizontal ist
        }
        return 0f; // Standardwinkel, wenn kein Boden gefunden wurde
    }

    private int currentSeedIndex = -1;

    public void SetCurrentSeedIndex(int index)
    {
        currentSeedIndex = index;
        Debug.Log("New Index: "+ currentSeedIndex);
        Debug.Log("All Seeds List: " + backpack.GetAllSeeds());
        Debug.Log("Target Seed: "+ backpack.GetAllSeeds()[currentSeedIndex]);
    }

    public void OnPlantingAnimation()
    {
        if (currentSeedIndex >= 0)
        {

            Seed seedToPlant = backpack.GetAndRemoveSeedAt(currentSeedIndex);
            if (seedToPlant != null)
            {
                Plant newPlant = new Plant((Plant.PlantType)seedToPlant.Type, seedToPlant.GrowthTime);

                float offsetY = 0.5f; // versatz nach unten
                float offsetX = 0.0f; // versatz L oder R

                if (isFacingRight)
                {
                    offsetX = 0.5f;
                }
                else
                {
                    offsetX = -0.5f;
                }

                Vector3 plantPosition = new Vector3(transform.position.x + offsetX, groundCheck.position.y + offsetY, transform.position.z); // Position von Pittis Füßen

                // Bestimmen Sie die Neigung des Bodens an der Pflanzposition
                float groundAngle = GetGroundAngle(plantPosition);

                // Erstellen Sie den Erdhügel und richten Sie ihn an der Neigung des Bodens aus
                GameObject plantInstance = Instantiate(plantPrefab, plantPosition, Quaternion.Euler(0, 0, groundAngle));

                // Starte die Wachstumsroutine
                StartCoroutine(GrowPlant(plantInstance, newPlant.Type, newPlant.GrowthTime));

                Debug.Log($"Planted a {newPlant.Type} seed with growth time of {newPlant.GrowthTime} seconds.");
                //Debug.Log($"Planted a {newPlant.Type} seed with growth time of {newPlant.GrowthTime} seconds.");
                //Debug.Log($"Plant position: {plantPosition}");
                //Debug.Log($"Pitti position: {transform.position}");
                //Debug.Log($"GroundCheck position: {groundCheck.position}");
            }
            currentSeedIndex = -1; // Zurücksetzen des Index nach dem Pflanzen
        }
    }

    private void HandleWateringAction(InputAction.CallbackContext context)
    {
        Debug.Log("Watering: ");
        animator.SetTrigger("HandleGoWatering");
    }

    // Hack um es aus dem Backback heraus zu umgehen
    private void HandlePrimaryAction(InputAction.CallbackContext context)
    {
        Debug.Log("HandlePickupDrop ausgeführt: ");
        HandlePickupDrop();
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
            case Plant.PlantType.Alocasia:
                newPlantPrefab = alocasiaPrefab;
                break;
            case Plant.PlantType.Croton:
                newPlantPrefab = crotonPrefab;
                break;
            case Plant.PlantType.Teaktree:
                newPlantPrefab = teaktreePrefab;
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
        float currentSpeed = isRunning ? speedFaster : speed;
        rb.velocity = new Vector2(horizontal * currentSpeed, rb.velocity.y);
    }

    private void Moving()
    {
        bool isMoving = Mathf.Abs(horizontal) > 0;
        animator.SetBool("IsRunning", isMoving);

        // Setze die Condition für die Running Fast Animation
        animator.SetBool("IsRunningFast", isRunning && isMoving);

        WalkSound(isMoving);
        Flip();
    }

    private void WalkSound(bool isMoving)
    {
        // Überprüfe, ob sich der Charakter bewegt und ob er rennt
        if (isMoving)
        {
            if (isRunning)
            {
                if (!runningSoundSource.isPlaying)
                {
                    walkSoundSource.Stop(); // Stelle sicher, dass der Walksound gestoppt ist
                    runningSoundSource.clip = runningSound;
                    runningSoundSource.loop = true;
                    runningSoundSource.Play();
                }
            }
            else
            {
                if (!walkSoundSource.isPlaying)
                {
                    runningSoundSource.Stop(); // Stelle sicher, dass der Runningsound gestoppt ist
                    walkSoundSource.clip = walkSound;
                    walkSoundSource.loop = true;
                    walkSoundSource.Play();
                }
            }
        }
        else
        {
            if (walkSoundSource.isPlaying)
            {
                walkSoundSource.Stop();
            }
            if (runningSoundSource.isPlaying)
            {
                runningSoundSource.Stop();
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
