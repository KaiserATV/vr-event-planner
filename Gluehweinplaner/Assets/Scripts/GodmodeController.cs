using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

public class GodmodeController : MonoBehaviour
{
    public GameObject xrRig; // XR Origin (Kamera + Controller)
    public float moveSpeed = 3f;  
    public float verticalSpeed = 2f; 

    private bool isGodmodeActive = false;

    // Input Actions für Trackpad-Steuerung
    public InputActionReference moveVerticalAction;
    public InputActionReference moveHorizontalAction;
    public InputActionReference toggleGodmodeAction;

    // Locomotion-System Komponenten
    public TeleportationProvider teleportProvider;
    public LocomotionProvider snapTurnProvider;

    private Vector3 originalPosition;

    void Update()
    {
        if (toggleGodmodeAction.action.WasPressedThisFrame())
        {
            ToggleGodmode();
        }

        if (isGodmodeActive)
        {
            MoveGodmode();
        }
    }

    void ToggleGodmode()
    {
        isGodmodeActive = !isGodmodeActive;

        if (isGodmodeActive)
        {
            // Speichere Position und verschiebe nach oben
            originalPosition = xrRig.transform.position;
            xrRig.transform.position += new Vector3(0, 5, 0);

            // Locomotion deaktivieren
            if (teleportProvider != null) teleportProvider.enabled = false;
            if (snapTurnProvider != null) snapTurnProvider.enabled = false;
        }
        else
        {
            // Zurück zur ursprünglichen Position
            xrRig.transform.position = originalPosition;

            // Locomotion wieder aktivieren
            if (teleportProvider != null) teleportProvider.enabled = true;
            if (snapTurnProvider != null) snapTurnProvider.enabled = true;
        }
    }

    void MoveGodmode()
    {
        float verticalMove = moveVerticalAction.action.ReadValue<float>() * verticalSpeed * Time.deltaTime;
        Vector2 horizontalInput = moveHorizontalAction.action.ReadValue<Vector2>();

        Vector3 move = new Vector3(horizontalInput.x, verticalMove, horizontalInput.y) * moveSpeed * Time.deltaTime;
        xrRig.transform.Translate(move, Space.Self);
    }
}
