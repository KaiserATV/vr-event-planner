using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class DebugTeleport : MonoBehaviour
{
    public TeleportationProvider teleportationProvider;

    void Start()
    {
        if (teleportationProvider == null)
        {
            teleportationProvider = FindObjectOfType<TeleportationProvider>();
        }
    }

    public void TriggerTeleport()
    {
        if (teleportationProvider != null)
        {
            TeleportRequest request = new TeleportRequest()
            {
                destinationPosition = new Vector3(0, 1, 0), // Test-Teleport über Boden
            };

            teleportationProvider.QueueTeleportRequest(request);
        }
        else
        {
            Debug.LogError("Kein Teleportation Provider gefunden!");
        }
    }
}
