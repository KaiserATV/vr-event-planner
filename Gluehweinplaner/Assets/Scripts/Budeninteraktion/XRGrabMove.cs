using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.XR;
using UnityEngine.XR.Interaction.Toolkit;

public class XRGrabMove : MonoBehaviour
{
    [Header("XR Interactor")]
    public XRBaseInteractor interactor; // Der XR Interactor (z. B. XRDirectInteractor oder XRRayInteractor)

    [Header("Input Actions")]
    public InputActionReference moveBuildingAction; // Bewegung (z. B. Trackpad oder Joystick)
    public InputActionReference rotateBuildingAction; // Rotation (z. B. Trackpad oder Joystick)

    private Transform grabbedObject;

    private void Update()
    {
        if (interactor != null && interactor.selectTarget != null)
        {
            grabbedObject = interactor.selectTarget.transform;

            // Eingabewerte abrufen (mit InputActionReference)
            Vector2 moveInput = moveBuildingAction.action?.ReadValue<Vector2>() ?? Vector2.zero;
            Vector2 rotateInput = rotateBuildingAction.action?.ReadValue<Vector2>() ?? Vector2.zero;

            // Objekt verschieben (X- und Z-Achse, Y bleibt gleich)
            Vector3 moveDelta = new Vector3(moveInput.x, 0, moveInput.y) * Time.deltaTime * 2f;
            grabbedObject.position += moveDelta;

            // Objekt rotieren (Drehen um die Y-Achse mit dem rechten/sekundären Stick)
            grabbedObject.Rotate(Vector3.up, rotateInput.x * 50f * Time.deltaTime);
        }
    }
}
