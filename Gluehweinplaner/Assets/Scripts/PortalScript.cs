using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.XR.Interaction.Toolkit;

public class PortalScript : MonoBehaviour
{
    public string targetScene;

    // Wird aufgerufen, wenn der Spieler das Portal auswählt
    public void OnPortalActivated()
    {
        if (!string.IsNullOrEmpty(targetScene))
        {
            SceneManager.LoadScene(targetScene);
        }
        else
        {
            Debug.LogError("Zielszene nicht angegeben!");
        }
    }
}