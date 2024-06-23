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
        public float zoomDuration = 2f;
        public float fadeDuration = 2f;
        public PostProcessVolume postProcessVolume; // Referenz zur Post-process Volume
        public PolygonCollider2D originalBounds; // Das ursprüngliche Bounding Shape
        public PolygonCollider2D expandedBounds; // Das erweiterte Bounding Shape
        public Collider2D stegBeiboot; // Der Collider für den Steg zum Beiboot
        public Collider2D stegHome; // Der Collider für den Steg zum Haus
        public PlayerMovement playerMovement; // Referenz zum PlayerMovement-Skript

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

        private IEnumerator GoodNightSequence()
        {
            isSceneActive = true;

            // Kamera auf Pitti zoomen
            yield return StartCoroutine(ZoomCamera(5f, zoomedInScreenY));

            // Bild dunkel werden lassen
            yield return StartCoroutine(FadeToBlack());

            // Collider wechseln
            stegBeiboot.enabled = false;
            stegHome.enabled = true;

            // Pitti zum Haus bewegen
            MovePittiToHouse();

            // Bild wieder hell werden lassen
            yield return StartCoroutine(FadeToClear());

            // Kamera zurückzoomen
            yield return StartCoroutine(ZoomCamera(9.96f, defaultScreenY));

            // Pitti automatisch ins Haus bewegen
            yield return StartCoroutine(MovePittiLeft(2f));
            Debug.Log("Auto MOve Ende! ");

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

        private IEnumerator FadeToBlack()
        {
            float elapsedTime = 0f;

            if (colorGrading != null)
            {
                while (elapsedTime < fadeDuration)
                {
                    colorGrading.postExposure.value = Mathf.Lerp(0f, -5f, (elapsedTime / fadeDuration)); // Exposition verringern
                    elapsedTime += Time.deltaTime;
                    yield return null;
                }

                colorGrading.postExposure.value = -5f;
            }
        }

        private void MovePittiToHouse()
        {
            pittiTransform.position = housePosition.position;
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

        private IEnumerator MovePittiLeft(float duration)
        {
            // Start automatic movement
            playerMovement.StartAutoMove(Vector2.left, duration);

            yield return new WaitForSeconds(duration);
        }

    }
}
