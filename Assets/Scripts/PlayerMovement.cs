﻿using Assets.Scripts.StoryElements;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    private float horizontal;
    private float speed = 1.5f; // walk
    private float speedFaster = 2.3f; // running
    private bool isRunning = false;
    private bool isFacingRight = true;
    private bool switchMovement = false; // Gibt an, ob der Charakter ein Objekt trägt
    private bool isPickingUp = false; // Zustand des Aufhebens/Ablegens
    private bool isAutoMoving = false;
    private Vector2 autoMoveDirection;

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
    [SerializeField] private GameObject wateringCanPrefab;
    [SerializeField] private GameObject waterRunningOutOfCanPrefab;
    [SerializeField] private GameObject mangrovePrefab; // Prefab für Mangrove
    [SerializeField] private GameObject farnPrefab;
    [SerializeField] private GameObject alocasiaPrefab;
    [SerializeField] private GameObject crotonPrefab;
    [SerializeField] private GameObject teaktreePrefab;
    [SerializeField] private Transform frontHandPosition;
    [SerializeField] private GameObject seedPrefab; // Generisches Samen-Prefab
    [SerializeField] private BeibootTrigger beibootTrigger; // Referenz zur SoundZone
    [SerializeField] private AudioSource pickupSoundSource; // AudioSource für das Aufheben
    [SerializeField] private AudioClip pickupSound; // Aufhebgeräusch
    [SerializeField] private AudioSource dropSoundSource; // AudioSource für das Ablegen
    [SerializeField] private AudioClip dropSound; // Standard Ableggeräusch
    [SerializeField] private List<AudioClip> beibootDropSounds; // Liste der Geräusche für die Beiboot-Zone

    // Denkbläsen-Prefabs
    [SerializeField] private GameObject nightThinkingBubblePrefab;
    [SerializeField] private GameObject plantsThinkingBubblePrefab;
    [SerializeField] private GameObject seedThinkingBubblePrefab;
    [SerializeField] private GameObject wateringCanThinkingBubblePrefab;
    [SerializeField] private GameObject trashThinkingBubblePrefab;

    // Die Position, an der die Denkbläse angezeigt werden soll
    [SerializeField] private Transform thinkingBubblePosition;


    // Diese Referenz wird im Start-Methodenblock automatisch gesetzt
    private PickupScript pickupScript;
    private Backpack backpack;
    private NightZone nightZone;
    private RotateObject skyDiscRotateObject;
    private GoodNightScene goodNightScene;
    private GameObject nearestObject;
    private GameObject seedInHand; // Referenz auf den Samen in Pittis Hand
    private GameObject wateringCanInHand;
    private GameObject waterRunningOutOfCan;

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
        input.Player.NightAction.performed += ctx => HandleNightAction();
    }

    public void DisableMovement()
    {
        input.Player.Move.performed -= OnMove;
        input.Player.Move.canceled -= OnMoveCanceled;
        input.Player.PrimaryAction.performed -= HandlePrimaryAction;
        input.Player.WateringAction.performed -= HandleWateringAction;

        horizontal = 0f; // Setze die horizontale Bewegung auf Null
        rb.velocity = Vector2.zero; // Setze die Geschwindigkeit auf Null
        input.Player.Move.Disable(); // Deaktiviere die Bewegungseingaben
        input.Player.RunningFaster.Disable(); // Deaktiviere die Lauf-Eingaben
    }

    public void EnableMovement()
    {
        StartCoroutine(EnableMovementAfterDelay());
    }

    private IEnumerator EnableMovementAfterDelay()
    {
        yield return new WaitForSeconds(0.2f);
        input.Player.Move.performed += OnMove;
        input.Player.Move.canceled += OnMoveCanceled;
        input.Player.PrimaryAction.performed += HandlePrimaryAction;
        input.Player.WateringAction.performed += HandleWateringAction;

        input.Player.Move.Enable(); // Aktiviere die Bewegungseingaben
        input.Player.RunningFaster.Enable(); // Aktiviere die Lauf-Eingaben
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
            backpack = gameObject.AddComponent<Backpack>();
        }

        nightZone = FindObjectOfType<NightZone>();

        if (nightZone == null)
        {
            Debug.LogError("Kein NightZone Script gefunden ");
        }

        GameObject skyDisc = GameObject.FindWithTag("SkyDisc");
        if (skyDisc != null)
        {
            skyDiscRotateObject = skyDisc.GetComponent<RotateObject>();
            if (skyDiscRotateObject == null)
            {
                Debug.LogError("RotateObject script not found on SkyDisc.");
            }
        }
        else
        {
            Debug.LogError("SkyDisc GameObject not found.");
        }

        goodNightScene = FindObjectOfType<GoodNightScene>();
        if (goodNightScene == null)
        {
            Debug.LogError("GoodNightScene script not found in the scene.");
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


    public void StartAutoMove(Vector2 direction, float duration)
    {
        isAutoMoving = true;
        autoMoveDirection = direction;
        isFacingRight = direction.x > 0;
        FlipAutoMove();
        animator.SetBool("IsRunning", true);
        StartCoroutine(StopAutoMoveAfterDuration(duration));
    }

    private IEnumerator StopAutoMoveAfterDuration(float duration)
    {
        yield return new WaitForSeconds(duration);
        isAutoMoving = false;
        animator.SetBool("IsRunning", false);
        horizontal = 0f; // Sicherstellen, dass Pitti stoppt
        rb.velocity = Vector2.zero; // Setze die Geschwindigkeit auf Null
    }



    public void OnPlantingAnimationSeedStart()
    {
        // Samen in die Hand positionieren
        DisableMovement();
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
        EnableMovement();
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
        Debug.Log("New Index: " + currentSeedIndex);
        Debug.Log("All Seeds List: " + backpack.GetAllSeeds());
        Debug.Log("Target Seed: " + backpack.GetAllSeeds()[currentSeedIndex]);
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
            }
            currentSeedIndex = -1; // Zurücksetzen des Index nach dem Pflanzen
        }
    }

    private void HandleWateringAction(InputAction.CallbackContext context)
    {
        DisableMovement();
        animator.SetTrigger("HandleGoWatering");

        // _________ Giesskanne
        // gießkannen prefab laden und animation abspielen
        wateringCanInHand = Instantiate(wateringCanPrefab, frontHandPosition.position, Quaternion.identity, frontHandPosition);

        // Rigidbody der Gießkanne finden und Schwerkraft deaktivieren
        Rigidbody2D wateringCanRigidbody = wateringCanInHand.GetComponent<Rigidbody2D>();
        if (wateringCanRigidbody != null)
        {
            wateringCanRigidbody.gravityScale = 0f; // Deaktiviere die Schwerkraft
            wateringCanRigidbody.velocity = Vector2.zero; // Setze die Geschwindigkeit auf Null
            wateringCanRigidbody.angularVelocity = 0f; // Setze die Drehgeschwindigkeit auf Null
        }

        // Setze die Gießkanne als Kind-Objekt der Handposition, um der Hand zu folgen
        wateringCanInHand.transform.parent = frontHandPosition;

        // Manuelle Eingabe der Position relativ zur Handposition
        wateringCanInHand.transform.localPosition = new Vector3(-0.8f, 0, 0);

        Invoke("SpawnWater", 0.1f);
    }


    private void SpawnWater()
    {
        // _________ Wasser
        waterRunningOutOfCan = Instantiate(waterRunningOutOfCanPrefab, wateringCanInHand.transform.position, Quaternion.identity);

        Rigidbody2D waterRigidbody = waterRunningOutOfCan.GetComponent<Rigidbody2D>();
        if (waterRigidbody != null)
        {
            waterRigidbody.gravityScale = 0f;
            waterRigidbody.velocity = Vector2.zero;
            waterRigidbody.angularVelocity = 0f;
        }

        waterRunningOutOfCan.transform.parent = wateringCanInHand.transform;
        waterRunningOutOfCan.transform.localScale *= 0.5f; // verkleinern
        waterRunningOutOfCan.transform.localPosition = new Vector3(-2.4f, 1.2f, 0);

        if (!isFacingRight) // Wasser horizontal spiegeln
        {
            Vector3 localScale = waterRunningOutOfCan.transform.localScale;
            localScale.x *= -1f;
            waterRunningOutOfCan.transform.localScale = localScale;
        }
    }

    public void OnWateringAnimationEnd()
    {
        if (wateringCanInHand != null)
        {
            Destroy(wateringCanInHand); // Entferne die Gießkanne aus Pittis Hand
        }
        if (waterRunningOutOfCan != null)
        {
            Destroy(waterRunningOutOfCan);
        }
        EnableMovement();
    }

    // Hack um es aus dem Backback heraus zu umgehen
    private void HandlePrimaryAction(InputAction.CallbackContext context)
    {
        HandlePickupDrop();
    }

    private void HandlePickupDrop()
    {
        // LAUFEn muss dekativert werden
        animator.SetTrigger("HandleGoDown");
        DisableMovement();

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
                    PlayPickupSound();
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

                if (beibootTrigger.IsPlayerInZone())
                {
                    PlayRandomBeibootDropSound(); // Spiele ein zufälliges Geräusch aus der Beiboot-Zone ab
                }
                else
                {
                    PlayDropSound(); // Spiele das Standard-Ableggeräusch ab
                }

                switchMovement = false;
                animator.SetBool("HasObject", false);
            }
        }
    }

    private void PlayPickupSound()
    {
        StartCoroutine(PlaySoundWithDelay(pickupSoundSource, pickupSound, 0.3f));
    }

    private void PlayDropSound()
    {
        StartCoroutine(PlaySoundWithDelay(dropSoundSource, dropSound, 0.1f));
    }

    private void PlayRandomBeibootDropSound()
    {
        if (beibootDropSounds.Count > 0)
        {
            int randomIndex = Random.Range(0, beibootDropSounds.Count);
            AudioClip randomClip = beibootDropSounds[randomIndex];
            StartCoroutine(PlaySoundWithDelay(dropSoundSource, randomClip, 0.5f));
        }
    }

    private IEnumerator PlaySoundWithDelay(AudioSource audioSource, AudioClip audioClip, float delay)
    {
        yield return new WaitForSeconds(delay);
        if (audioSource != null && audioClip != null)
        {
            audioSource.PlayOneShot(audioClip);
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
                    nearestObject.SetActive(false); // Samen deaktivieren, nachdem er aufgesammelt wurde
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
        EnableMovement();
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

    private void HandleNightAction()
    {
        if (skyDiscRotateObject != null)
        {

            if(!nightZone.hasTriggeredNightEvent) // Ist es noch nicht dunkel
            {
                if (!skyDiscRotateObject.rotateToNightZone) // in rotation
                {
                    Debug.Log("Rotating to the Night Zone.");
                    DisableMovement();
                    skyDiscRotateObject.StartRotationToNightZone();
                }
            }
            else // Nacht einleiten
            {
                // TODO: ENABLEMovement ausführen am ende der Scene
                Debug.Log("Good Noght Scene");
                goodNightScene.StartGoodNightSequence();
            }
        }
    }


    public void ShowNightThinkingBubble()
    {
        if (nightThinkingBubblePrefab != null && thinkingBubblePosition != null)
        {
            Instantiate(nightThinkingBubblePrefab, thinkingBubblePosition.position, Quaternion.identity, thinkingBubblePosition);
        }
    }


    private void FixedUpdate()
    {
        float currentSpeed = isRunning ? speedFaster : speed;

        if (isAutoMoving)
        {
            rb.velocity = new Vector2(autoMoveDirection.x * currentSpeed, rb.velocity.y);
        }
        else
        {
            rb.velocity = new Vector2(horizontal * currentSpeed, rb.velocity.y);
        }
        Moving();
    }


    private void Moving()
    {
        bool isMoving = isAutoMoving ? true : Mathf.Abs(horizontal) > 0;
        animator.SetBool("IsRunning", isMoving);

        // Setze die Condition für die Running Fast Animation
        animator.SetBool("IsRunningFast", isRunning && isMoving);

        WalkSound(isMoving);
        if (isAutoMoving)
        {
            // Stelle sicher, dass Pitti in die richtige Richtung schaut
            FlipAutoMove();
        }
        else
        {
            Flip();
        }
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

    private void FlipAutoMove()
    {
        if (isFacingRight && transform.localScale.x < 0 || !isFacingRight && transform.localScale.x > 0)
        {
            Vector3 localScale = transform.localScale;
            localScale.x *= -1f;
            transform.localScale = localScale;
        }
    }

}
