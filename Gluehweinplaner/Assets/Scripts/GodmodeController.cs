using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;
using System.Collections;

public class GodmodeController : MonoBehaviour
{
    public GameObject xrRig; // XR Origin (Kamera + Controller)
    public float moveSpeed = 3f;
    public float verticalSpeed = 2f;
    public float transitionDuration = 1f;

    private bool isGodmodeActive = false;

    // Input Actions für Trackpad-Steuerung
    public InputActionReference moveVerticalAction;
    public InputActionReference moveHorizontalAction;
    public InputActionReference toggleGodmodeAction;

    // Locomotion-System Komponenten
    public TeleportationProvider teleportProvider;
    public LocomotionProvider snapTurnProvider;

    private Vector3 originalPosition;

    [SerializeField] private AudioClip godmodeActivateSound;
    [SerializeField] private AudioClip godmodeDeactivateSound;

   void Update()
    {
        if (toggleGodmodeAction.action.WasPressedThisFrame())
        {
            StartCoroutine(ToggleGodmode());
        }

        if (isGodmodeActive)
        {
            MoveGodmode();
        }
    }

    public IEnumerator ToggleGodmode()
    {
        isGodmodeActive = !isGodmodeActive;

        // Play sound effect
        if (isGodmodeActive)
        {
            if(godmodeActivateSound != null)
                SoundFXManager.instance.PlaySoundFXClip(godmodeActivateSound, transform, 1f);
        }
        else
        {
            if(godmodeDeactivateSound != null)
                SoundFXManager.instance.PlaySoundFXClip(godmodeDeactivateSound, transform, 1f);
        }

        if (isGodmodeActive)
        {
            originalPosition = xrRig.transform.position;
            Vector3 targetPosition = originalPosition + new Vector3(0, 5, 0);
            yield return StartCoroutine(SmoothTransition(targetPosition));

            if (teleportProvider != null) teleportProvider.enabled = false;
            if (snapTurnProvider != null) snapTurnProvider.enabled = false;
        }
        else
        {
            yield return StartCoroutine(SmoothTransition(originalPosition));

            if (teleportProvider != null) teleportProvider.enabled = true;
            if (snapTurnProvider != null) snapTurnProvider.enabled = true;
        }
    }

    IEnumerator SmoothTransition(Vector3 targetPosition)
    {
        Vector3 startPosition = xrRig.transform.position;
        float elapsedTime = 0f;

        while (elapsedTime < transitionDuration)
        {
            xrRig.transform.position = Vector3.Lerp(startPosition, targetPosition, elapsedTime / transitionDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        
        xrRig.transform.position = targetPosition;
    }

    void MoveGodmode()
    {
        float verticalMove = moveVerticalAction.action.ReadValue<float>() * verticalSpeed * Time.deltaTime;
        Vector2 horizontalInput = moveHorizontalAction.action.ReadValue<Vector2>();

        Vector3 forward = new Vector3(xrRig.transform.forward.x, 0, xrRig.transform.forward.z).normalized;
        Vector3 right = new Vector3(xrRig.transform.right.x, 0, xrRig.transform.right.z).normalized;
        Vector3 horizontalMove = (forward * horizontalInput.y + right * horizontalInput.x) * moveSpeed * Time.deltaTime;
        Vector3 verticalMovement = Vector3.up * verticalMove;

        xrRig.transform.position += horizontalMove + verticalMovement;
    }
}
