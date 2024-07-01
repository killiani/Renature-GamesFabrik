using System.Collections;
using UnityEngine;
using Cinemachine;
using UnityEngine.Rendering.PostProcessing;

namespace Assets.Scripts.StoryElements
{
    public class GoodNightScene : MonoBehaviour
    {
        public CinemachineVirtualCamera followCamera; // Referenz zur Cinemachine Virtual Camera
        public Transform pittiTransform;
        public Transform housePosition;
        public GameObject houseLayer;
        public BoxCollider2D houseTrigger; // Collider der als Trigger fungiert
        public BoxCollider2D startDayTrigger;
        public float zoomDuration = 2f;
        public float fadeDuration = 2f;
        public PostProcessVolume postProcessVolume; // Referenz zur Post-process Volume
        public PolygonCollider2D originalBounds; // Das ursprüngliche Bounding Shape
        public PolygonCollider2D expandedBounds; // Das erweiterte Bounding Shape
        public Collider2D stegBeiboot; // Der Collider für den Steg zum Beiboot
        public Collider2D stegHome; // Der Collider für den Steg zum Haus
        public PlayerMovement playerMovement; // Referenz zum PlayerMovement-Skript
        private NightZone nightZone;
        private HouseTrigger houseTriggerScript;
        private StartDayTrigger startDayTriggerScript;

        private bool isSceneActive = false;
        private ColorGrading colorGrading;
        private CinemachineFramingTransposer framingTransposer;
        private CinemachineConfiner2D confiner2D;
        public float defaultScreenY = 0.94f; // Der Standardwert für Screen Y
        public float zoomedInScreenY = 0.5f; // Der Screen Y Wert für das Hineinzoomen

        void Start()
        {
            if (followCamera == null)
            {
                Debug.LogError("Cinemachine Virtual Camera is not assigned.");
            }
            else
            {
                framingTransposer = followCamera.GetCinemachineComponent<CinemachineFramingTransposer>();
                confiner2D = followCamera.GetComponent<CinemachineConfiner2D>();
                if (framingTransposer == null)
                {
                    Debug.LogError("Cinemachine Framing Transposer is not assigned.");
                }
                if (confiner2D == null)
                {
                    Debug.LogError("Cinemachine Confiner 2D is not assigned.");
                }
            }

            nightZone = FindObjectOfType<NightZone>();
            if (nightZone == null)
            {
                Debug.LogError("NightZone script not found in the scene.");
            }

            houseTriggerScript = houseTrigger.GetComponent<HouseTrigger>();
            if (houseTriggerScript == null)
            {
                Debug.LogError("HouseTrigger script not found on houseTrigger object.");
            }

            startDayTriggerScript = startDayTrigger.GetComponent<StartDayTrigger>();
            if (startDayTriggerScript == null)
            {
                Debug.LogError("StartDayTrigger script not found on startDayTrigger object.");
            }

            if (postProcessVolume == null)
            {
                Debug.LogError("Post-process Volume is not assigned.");
            }
            else
            {
                postProcessVolume.profile.TryGetSettings(out colorGrading);
            }

            if (originalBounds == null)
            {
                Debug.LogError("Original bounds not assigned.");
            }

            if (expandedBounds == null)
            {
                Debug.LogError("Expanded bounds not assigned.");
            }

            if (stegBeiboot == null || stegHome == null)
            {
                Debug.LogError("Steg colliders not assigned.");
            }

            if (playerMovement == null)
            {
                Debug.LogError("PlayerMovement script not assigned.");
            }
        }

        void Update()
        {
            if (isSceneActive)
            {
                // Kamera-Logik und andere Dinge können hier platziert werden
            }
        }

        public void StartGoodNightSequence()
        {
            StartCoroutine(GoodNightSequence());
        }

        void ChangeLayer(GameObject obj, int layer)
        {
            SpriteRenderer spriteRenderer = obj.GetComponent<SpriteRenderer>();
            if (spriteRenderer != null)
            {
                spriteRenderer.sortingOrder = layer;
            }
            else
            {
                Debug.LogWarning("The object does not have a SpriteRenderer component.");
            }
        }

        private IEnumerator GoodNightSequence()
        {
            isSceneActive = true;

            // Kamera auf Pitti zoomen
            // yield return StartCoroutine(ZoomCamera(5f, zoomedInScreenY));

            // Bild dunkel werden lassen
            // yield return StartCoroutine(FadeTo(0f, -5f, 0f));

            // Collider wechseln
            stegBeiboot.enabled = false;
            stegHome.enabled = true;

            // Haus Layer wechseln
            ChangeLayer(houseLayer, 9);

            // Pitti zum Haus bewegen
            yield return StartCoroutine(MovePittiToHouse());

            // Bild wieder hell werden lassen
            //yield return StartCoroutine(FadeToClear());

            // Pitti automatisch ins Haus bewegen
            //yield return StartCoroutine(MovePitti(2f, Vector2.left));

            // Tag rotieren
            if (nightZone != null)
            {
                nightZone.TriggerDayEvent();
            }

            // Bild dunkel werden lassen
            yield return StartCoroutine(FadeTo(0f, -10f, 0f));

            // Alle Erdhuegel werden zu Pflanzen
            playerMovement.GrowAllWateredHillsOvernight();

            // Alle gewaesserten Pflanzen geben Samen
            playerMovement.GrowAllSeedsOvernight();

            // Bild wieder hell werden lassen - nächster Morgen
            yield return StartCoroutine(FadeTo(-10f, 0f, 0f));
            //yield return StartCoroutine(FadeToClear());

            // Pitti automatisch aus dem Haus bewegen
            yield return StartCoroutine(MovePittiToStartDay());


            // Collider wechseln
            stegBeiboot.enabled = true;
            stegHome.enabled = false;

            // Haus Layer wechseln
            ChangeLayer(houseLayer, 1);

            // Kamera zurückzoomen
            // yield return StartCoroutine(ZoomCamera(9.96f, defaultScreenY));

            // Bewegung aktivieren
            playerMovement.EnableMovement();
            playerMovement.DisableNightAction();

            nightZone.hasTriggeredNightEvent = false; // zuruecksetzen

            isSceneActive = false;
        }

        private IEnumerator ZoomCamera(float targetOrthoSize, float targetScreenY)
        {
            float startOrthoSize = followCamera.m_Lens.OrthographicSize;
            float elapsedTime = 0f;

            float startScreenY = framingTransposer.m_ScreenY;

            // Wechsel zu den erweiterten Begrenzungen während des Zoomens
            confiner2D.m_BoundingShape2D = expandedBounds;

            while (elapsedTime < zoomDuration)
            {
                followCamera.m_Lens.OrthographicSize = Mathf.Lerp(startOrthoSize, targetOrthoSize, (elapsedTime / zoomDuration));
                framingTransposer.m_ScreenY = Mathf.Lerp(startScreenY, targetScreenY, (elapsedTime / zoomDuration));
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            followCamera.m_Lens.OrthographicSize = targetOrthoSize;
            framingTransposer.m_ScreenY = targetScreenY;

            // Wechsel zurück zu den ursprünglichen Begrenzungen nach dem Zoom
            confiner2D.m_BoundingShape2D = originalBounds;
        }

        private IEnumerator MovePittiToHouse()
        {
            Vector2 direction = (houseTrigger.transform.position - pittiTransform.position).normalized;
            playerMovement.StartAutoMove(direction, true);

            // Warte, bis Pitti den houseTrigger erreicht hat
            while (!houseTriggerScript.IsPlayerInZone())
            {
                //Debug.Log("Moving towards house...");
                yield return null;
            }

            //Debug.Log("Pitti reached the house!");
            playerMovement.StopAutoMove();
        }

        private IEnumerator MovePittiToStartDay()
        {
            Vector2 direction = (startDayTrigger.transform.position - pittiTransform.position).normalized;
            playerMovement.StartAutoMove(direction, false);

            // Warte, bis Pitti den startDayTrigger erreicht hat
            while (!startDayTriggerScript.IsPlayerInZone())
            {
                //Debug.Log("Moving towards start day...");
                yield return null;
            }

            //Debug.Log("Pitti reached the start day position!");
            playerMovement.StopAutoMove();
        }

        private IEnumerator FadeTo(float from, float to, float time)
        {
            float elapsedTime = time;

            if (colorGrading != null)
            {
                while (elapsedTime < fadeDuration)
                {
                    colorGrading.postExposure.value = Mathf.Lerp(from, to, (elapsedTime / fadeDuration)); // Exposition verringern
                    elapsedTime += Time.deltaTime;
                    yield return null;
                }

                colorGrading.postExposure.value = to;
            }
        }
        private IEnumerator FadeToClear()
        {
            float elapsedTime = 0f;

            if (colorGrading != null)
            {
                while (elapsedTime < fadeDuration)
                {
                    colorGrading.postExposure.value = Mathf.Lerp(-5f, 0f, (elapsedTime / fadeDuration)); // Exposition wieder erhöhen
                    elapsedTime += Time.deltaTime;
                    yield return null;
                }

                colorGrading.postExposure.value = 0f;
            }
        }
    }
}
