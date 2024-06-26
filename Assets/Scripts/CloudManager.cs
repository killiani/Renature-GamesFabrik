using System.Collections;
using UnityEngine;

public class CloudManager : MonoBehaviour
{
    public GameObject[] cloudPrefabs; // Array von Wolken-Prefabs
    public float initialSpawnFrequency = 15f; // Anfangs-Zeitintervall zwischen Spawns
    public Vector2 spawnHeightRange = new Vector2(4.42f, 8.03f); // Höhe, in der die Wolken spawnen
    public float cloudSpeed = 0.61f; // Geschwindigkeit der Wolkenbewegung
    public PolygonCollider2D worldBounds; // Referenz zum PolygonCollider2D der Welt

    private float nextSpawnTime;
    private float spawnFrequency;
    private float elapsedTime;

    void Start()
    {
        if (worldBounds == null)
        {
            Debug.LogError("World bounds not assigned.");
        }
        spawnFrequency = initialSpawnFrequency;
        nextSpawnTime = Time.time + spawnFrequency;
        StartCoroutine(UpdateSpawnFrequency());
    }

    void Update()
    {
        if (Time.time >= nextSpawnTime)
        {
            SpawnCloud();
            nextSpawnTime = Time.time + spawnFrequency;
        }
    }

    void SpawnCloud()
    {
        // Bestimme die rechte und linke Kante des PolygonColliders
        Vector2[] points = worldBounds.points;
        float minX = float.MaxValue;
        float maxX = float.MinValue;

        foreach (var point in points)
        {
            Vector3 worldPoint = worldBounds.transform.TransformPoint(point);
            if (worldPoint.x < minX) minX = worldPoint.x;
            if (worldPoint.x > maxX) maxX = worldPoint.x;
        }

        float spawnX = maxX;
        float despawnX = minX;
        float spawnY = Random.Range(spawnHeightRange.x, spawnHeightRange.y);
        Vector3 spawnPosition = new Vector3(spawnX, spawnY, 0);
        GameObject cloud = Instantiate(cloudPrefabs[Random.Range(0, cloudPrefabs.Length)], spawnPosition, Quaternion.identity);
        cloud.AddComponent<CloudMover>().Initialize(cloudSpeed, despawnX);
    }

    private IEnumerator UpdateSpawnFrequency()
    {
        while (true)
        {
            yield return new WaitForSeconds(60f); // Warte eine Minute

            elapsedTime += 60f;

            if (elapsedTime == 60f)
            {
                spawnFrequency = 10f; // Nach 1 Minute
            }
            else if (elapsedTime == 120f)
            {
                spawnFrequency = 5f; // Nach 2 Minuten
            }
            else if (elapsedTime == 180f)
            {
                spawnFrequency = 3f; // Nach 3 Minuten
                elapsedTime = 0f; // Zurücksetzen des Timers nach 3 Minuten
                spawnFrequency = initialSpawnFrequency; // Wieder auf Anfangsfrequenz
            }
        }
    }
}

public class CloudMover : MonoBehaviour
{
    private float speed;
    private float despawnXPosition;

    public void Initialize(float cloudSpeed, float despawnPosition)
    {
        speed = cloudSpeed;
        despawnXPosition = despawnPosition;
    }

    void Update()
    {
        transform.Translate(Vector3.left * speed * Time.deltaTime);

        if (transform.position.x <= despawnXPosition)
        {
            Destroy(gameObject);
        }
    }
}
