using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;
using System.Collections;

public class GodmodeController : MonoBehaviour
{
    public GameObject xrRig;
    public float moveSpeed = 3f;
    public float verticalSpeed = 2f;
    public float transitionDuration = 1f;

    private bool isGodmodeActive = false;
    private bool isGrabbingObject = false;
    private bool isPlacingObject = false;

    public InputActionReference verticalMoveAction;
    public InputActionReference horizontalMoveAction;
    public InputActionReference toggleGodmodeAction;
    public InputActionReference grabAction;

    public TeleportationProvider teleportProvider;
    public LocomotionProvider snapTurnProvider;
    public XRRayInteractor teleportRayInteractor; // Neuer Teleport Ray Interactor
    
    public GameObject vignetteEffect;
    private Vector3 originalPosition;

    void Start()
    {
        if (grabAction != null)
        {
            grabAction.action.started += _ => StartGrabbing();
            grabAction.action.canceled += _ => StopGrabbing();
        }
    }

    void Update()
    {
        if (toggleGodmodeAction.action.WasPressedThisFrame())
        {
            StartCoroutine(ToggleGodmode());
        }

        if (isGodmodeActive && !isGrabbingObject && !isPlacingObject)
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

        if (teleportRayInteractor != null)
        {
            teleportRayInteractor.enabled = !isGodmodeActive;
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
        if (!isGrabbingObject && !isPlacingObject)
        {
            float verticalMove = verticalMoveAction.action.ReadValue<float>() * verticalSpeed * Time.deltaTime;
            Vector3 verticalMovement = Vector3.up * verticalMove;

            Vector2 horizontalInput = horizontalMoveAction.action.ReadValue<Vector2>();
            Transform cameraTransform = Camera.main.transform;

            Vector3 moveDirection = (cameraTransform.forward * horizontalInput.y + cameraTransform.right * horizontalInput.x);
            moveDirection.y = 0;
            moveDirection.Normalize();

            Vector3 horizontalMove = moveDirection * moveSpeed * Time.deltaTime;

            xrRig.transform.position += horizontalMove + verticalMovement;
        }
    }

    void StartGrabbing()
    {
        isGrabbingObject = true;
    }

    void StopGrabbing()
    {
        isGrabbingObject = false;
    }

    public void StartPlacingObject()
    {
        isPlacingObject = true;
    }

    public void StopPlacingObject()
    {
        isPlacingObject = false;
    }
}
