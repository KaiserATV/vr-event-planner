using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using Valve.VR;

public class ViveTeleportInput : MonoBehaviour
{
    public SteamVR_Action_Vector2 touchpadAction;
    public SteamVR_Input_Sources handType = SteamVR_Input_Sources.RightHand;

    public GameObject teleportRayObject;  // XR Ray Interactor als GameObject referenzieren
    public TeleportationProvider teleportationProvider;

    private bool isTeleporting = false;

    void Update()
    {
        Vector2 touchpadValue = touchpadAction.GetAxis(handType);
       

        if (touchpadValue.y > 0.5f) // Touchpad nach vorne gedrückt
        {
            if (!isTeleporting)
            {
                teleportRayObject.SetActive(true); // XR Ray Interactor aktivieren
                isTeleporting = true;
            }
        }
        else
        {
            if (isTeleporting)
            {
                teleportRayObject.SetActive(false); // XR Ray Interactor deaktivieren
                isTeleporting = false;
            }
        }
    }

    public void PerformTeleport(SelectEnterEventArgs args)
    {
        TeleportationAnchor anchor = args.interactableObject.transform.GetComponent<TeleportationAnchor>();

        if (anchor != null)
        {
            TeleportRequest request = new TeleportRequest()
            {
                destinationPosition = anchor.teleportAnchorTransform.position
            };
            teleportationProvider.QueueTeleportRequest(request);
        }
    }
}
