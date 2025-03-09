using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

public class XRGrabVelocityControl : MonoBehaviour
{
    [Header("XR Interactor")]
    public XRBaseInteractor interactor;

    [Header("Input Actions")]
    public InputActionReference rotateBuildingAction; // Nutzt Trackpad Position (nicht Click!)

    [Header("Physik-Einstellungen")]
    public float rotationSpeed = 100f;

    private Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            Debug.LogError("Kein Rigidbody gefunden! Füge einen Rigidbody zum Objekt hinzu.");
        }
    }

    private void Update()
    {
        if (interactor != null && interactor.selectTarget == this.GetComponent<XRGrabInteractable>())
        {
            // **NEU:** Stelle sicher, dass eine 2D-Achse gelesen wird (statt Button)
            Vector2 rotateInput = Vector2.zero;

            if (rotateBuildingAction != null && rotateBuildingAction.action != null)
            {
                rotateInput = rotateBuildingAction.action.ReadValue<Vector2>(); 
            }

            // Rotation über Angular Velocity setzen
            if (rb != null)
            {
                rb.angularVelocity = new Vector3(0, rotateInput.x * rotationSpeed * Time.deltaTime, 0);
            }
        }
    }
}
