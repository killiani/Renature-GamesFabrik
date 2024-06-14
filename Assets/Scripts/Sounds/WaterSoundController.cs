using UnityEngine;

public class WaterSoundController : MonoBehaviour
{
    public Transform character; // Referenz zu Ihrem Charakter
    public Transform oceanPosition; // Referenz zu Ihrem OzeanPosition-Objekt
    public AudioSource oceanAudioSource; // Referenz zu der AudioSource
    public float maxVolume = 1.0f; // Maximale Lautstärke
    public float maxDistance = 10.0f; // Distanz, bei der der Sound auf maxVolume ist

    void Start()
    {
        if (oceanAudioSource == null)
        {
            Debug.LogError("Keine AudioSource am Ozean-Objekt gefunden.");
        }
    }

    void Update()
    {
        if (oceanAudioSource != null && character != null && oceanPosition != null)
        {
            float distance = Vector3.Distance(character.position, oceanPosition.position);
            float volume = Mathf.Clamp(1 - (distance / maxDistance), 0, maxVolume);
            oceanAudioSource.volume = volume;
        }
    }
}
