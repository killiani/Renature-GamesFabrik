using UnityEngine;

public class GameExit : MonoBehaviour
{
    public void ExitGame()
    {
        // Drucke eine Nachricht in der Konsole (nur im Editor sichtbar)
        Debug.Log("Exit Game");

        #if UNITY_EDITOR
                // Diese Zeile beendet den Spielmodus im Editor.
                UnityEditor.EditorApplication.isPlaying = false;
        #else
                // Beendet das Spiel in der gebauten Version.
                Application.Quit();
        #endif
    }
}
