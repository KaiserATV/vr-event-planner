using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
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
    public XRRayInteractor teleportRayInteractor;

    public Volume postProcessingVolume;
    private Vignette vignette;
    
    private Vector3 originalPosition;
    private float maxDistance = 10f; // Maximale Distanz für Vignette

    void Start()
    {
        if (grabAction != null)
        {
            grabAction.action.started += _ => StartGrabbing();
            grabAction.action.canceled += _ => StopGrabbing();
        }

        if (postProcessingVolume != null)
        {
            postProcessingVolume.profile.TryGet(out vignette);
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
            UpdateVignetteIntensity();
        }
    }

    public IEnumerator ToggleGodmode()
    {
        isGodmodeActive = !isGodmodeActive;

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
            StartCoroutine(StrongVignetteFadeOut());
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

    void UpdateVignetteIntensity()
    {
        if (vignette == null) return;

        float distance = Vector3.Distance(originalPosition, xrRig.transform.position);
        float intensity = Mathf.Lerp(0.2f, 0.8f, distance / maxDistance); // Stärkere Vignette mit Distanz

        vignette.intensity.value = intensity;
    }

    IEnumerator StrongVignetteFadeOut()
    {
        if (vignette == null) yield break;

        vignette.intensity.value = 1f; // Setzt Vignette auf sehr stark
        yield return new WaitForSeconds(0.3f); // Kurze starke Abdunklung

        float elapsedTime = 0f;
        float startIntensity = vignette.intensity.value;

        while (elapsedTime < 1f)
        {
            vignette.intensity.value = Mathf.Lerp(startIntensity, 0.2f, elapsedTime / 1f);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        vignette.intensity.value = 0.2f; // Setzt sie wieder auf normalen Wert
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
