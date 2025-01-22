using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class CustomNearFarGrabHandler : MonoBehaviour
{
    private UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable grabInteractable;
    private UnityEngine.XR.Interaction.Toolkit.Interactors.IXRSelectInteractor rightHand;
    private UnityEngine.XR.Interaction.Toolkit.Interactors.IXRSelectInteractor leftHand;

    void Start()
    {
        grabInteractable = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();
        grabInteractable.selectEntered.AddListener(OnGrab);
        grabInteractable.selectExited.AddListener(OnRelease);
    }

    void OnGrab(SelectEnterEventArgs args)
    {
        // Prüfen, ob der Interactor von der rechten oder linken Hand kommt
        if (args.interactorObject.transform.gameObject.name.Contains("Right"))
        {
            rightHand = args.interactorObject;
        }
        else if (args.interactorObject.transform.gameObject.name.Contains("Left"))
        {
            leftHand = args.interactorObject;
        }
    }

    void OnRelease(SelectExitEventArgs args)
    {
        if (args.interactorObject == rightHand)
        {
            rightHand = null;
        }
        else if (args.interactorObject == leftHand)
        {
            leftHand = null;
        }
    }

    void Update()
    {
        if (rightHand != null)
        {
            // Nur Position auf X/Z bewegen (Höhe bleibt gleich)
            Vector3 newPos = new Vector3(rightHand.transform.position.x, transform.position.y, rightHand.transform.position.z);
            transform.position = newPos;
        }

        if (leftHand != null)
        {
            // Rotation nur um die Y-Achse
            transform.rotation = Quaternion.Euler(0, leftHand.transform.eulerAngles.y, 0);
        }
    }
}
