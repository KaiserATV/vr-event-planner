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
    private bool isGrabbingObject = false;

    // Input Actions für Steuerung
    public InputActionReference verticalMoveAction; // Nur linker Controller für vertikale Bewegung
    public InputActionReference horizontalMoveAction; // Nur rechter Controller für horizontale Bewegung
    public InputActionReference toggleGodmodeAction;
    public InputActionReference grabAction;

    // Locomotion-System Komponenten
    public TeleportationProvider teleportProvider;
    public LocomotionProvider snapTurnProvider;
    
    public GameObject vignetteEffect; // Schwarze Vignette gegen Motion Sickness

    private Vector3 originalPosition;

    void Start()
    {
        if (grabAction != null)
        {
            grabAction.action.started += _ => isGrabbingObject = true;
            grabAction.action.canceled += _ => isGrabbingObject = false;
        }
    }

    void Update()
    {
        if (toggleGodmodeAction.action.WasPressedThisFrame())
        {
            StartCoroutine(ToggleGodmode());
        }

        if (isGodmodeActive && !isGrabbingObject)
        {
            MoveGodmode();
        }
    }

    public IEnumerator ToggleGodmode()
    {
        isGodmodeActive = !isGodmodeActive;

        if (vignetteEffect != null)
        {
            vignetteEffect.SetActive(isGodmodeActive);
        }

        if (isGodmodeActive)
        {
            originalPosition = xrRig.transform.position;
            Vector3 targetPosition = originalPosition + Vector3.up * 5;
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
        // Vertikale Bewegung nur über linken Controller (y-Achse)
        float verticalMove = verticalMoveAction.action.ReadValue<float>() * verticalSpeed * Time.deltaTime;
        Vector3 verticalMovement = Vector3.up * verticalMove;

        // Horizontale Bewegung nur über rechten Controller (x- und y-Achse, aber ohne Höhenveränderung)
        Vector2 horizontalInput = horizontalMoveAction.action.ReadValue<Vector2>();
        Transform cameraTransform = Camera.main.transform;
        
        Vector3 moveDirection = (cameraTransform.forward * horizontalInput.y + cameraTransform.right * horizontalInput.x);
        moveDirection.y = 0; // Höhenveränderung verhindern
        moveDirection.Normalize();

        Vector3 horizontalMove = moveDirection * moveSpeed * Time.deltaTime;
        
        xrRig.transform.position += horizontalMove + verticalMovement;
    }
}