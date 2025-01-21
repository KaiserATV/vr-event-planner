using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.XR.Interaction.Toolkit;

public class PortalScript : MonoBehaviour
{
    public string targetScene;

    // Wird aufgerufen, wenn das Portal ausgewählt wird
    public void TeleportToScene()
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
