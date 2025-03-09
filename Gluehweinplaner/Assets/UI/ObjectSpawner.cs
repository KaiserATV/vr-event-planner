using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ObjectSpawner : MonoBehaviour
{
    public List<GameObject> objectPrefabs;
    public float placementDistance = 2f;
    public Material previewMaterial;
    public GameObject budenContainer;
    public AgentManager am;
    public GodmodeController godmodeController; // Nur noch GodmodeController

    private GameObject currentPreview;
    private int selectedIndex = -1;

    public Transform handTransform;
    public Transform cameraTransform;

    public InputActionReference moveAction;
    public InputActionReference rotateAction;
    public InputActionReference confirmAction;

    private bool isPlacing = false;
    public bool IsPlacing => isPlacing;

    private Vector3 placementPosition;
    private float placementRotationY = 0f;
    private float rotationSpeed = 100f;
    private bool hasManualRotation = false;
    private bool hasUsedMoveInput = false;
private float lastRotationInput = 0f;

    void Start()
    {
        budenContainer = GameObject.Find("BudenContainer");    
        am = GameObject.Find("AgentManager").GetComponent<AgentManager>();
        cameraTransform = Camera.main.transform;

        godmodeController = FindObjectOfType<GodmodeController>();

        if (godmodeController == null)
            Debug.LogError("GodmodeController wurde nicht gefunden!");
    }

    public void StartSpawning(int index)
    {
        if (index < 0 || index >= objectPrefabs.Count) return;

        isPlacing = true;
        selectedIndex = index;
        CreatePreviewObject();

        // Bewegung im Godmode deaktivieren
        if (godmodeController != null && godmodeController.IsGodmodeActive())
        {
            godmodeController.StartPlacingObject();
        }

        Vector3 forwardOffset = cameraTransform.forward * placementDistance;
        placementPosition = new Vector3(forwardOffset.x, 0, forwardOffset.z) + cameraTransform.position;
        placementRotationY = cameraTransform.eulerAngles.y;
    }

    void Update()
{
    if (!isPlacing || currentPreview == null) return;

    Vector3 handOffset = handTransform.position + handTransform.forward * placementDistance;
    Vector3 newPosition = new Vector3(handOffset.x, 0, handOffset.z);
    float handRotationY = handTransform.eulerAngles.y;

    Vector2 moveInput = moveAction.action.ReadValue<Vector2>();
    float rotationInput = rotateAction.action.ReadValue<float>();

    // **Bewegung relativ zur Blickrichtung berechnen**
    if (moveInput.magnitude > 0.01f) 
    {
        Vector3 forward = cameraTransform.forward;
        Vector3 right = cameraTransform.right;

        forward.y = 0; right.y = 0;
        forward.Normalize();
        right.Normalize();

        Vector3 movement = (right * moveInput.x + forward * moveInput.y) * Time.deltaTime * 2f;
        placementPosition += movement;

        hasUsedMoveInput = true;
    }
    else if (!hasUsedMoveInput)
    {
        placementPosition = newPosition;
    }

    // **Drehrichtung speichern**
    if (Mathf.Abs(rotationInput) > 0.01f) 
    {
        lastRotationInput = rotationInput; // Speichert die Richtung der letzten Eingabe
    }

    // **Rotation - Kombinierte Steuerung (Klick & Halten)**
    if (rotateAction.action.WasPressedThisFrame()) 
    {
        // Nutze die letzte gespeicherte Eingabe, um die Richtung zu bestimmen
        if (lastRotationInput > 0) 
        {
            placementRotationY += 5f; // Drehung nach rechts
        }
        else if (lastRotationInput < 0) 
        {
            placementRotationY -= 5f; // Drehung nach links
        }

        hasManualRotation = true;
    }
    else if (Mathf.Abs(rotationInput) > 0.01f) 
    {
        // Falls die Taste gehalten wird, rotiere fließend
        placementRotationY += rotationInput * rotationSpeed * Time.deltaTime;
        hasManualRotation = true;
    }
    else if (!hasManualRotation) 
    {
        placementRotationY = handRotationY;
    }

    // **Vorschau-Objekt aktualisieren**
    currentPreview.transform.position = placementPosition;
    currentPreview.transform.rotation = Quaternion.Euler(0, placementRotationY, 0);

    if (confirmAction.action.triggered)
    {
        PlaceObject();
        isPlacing = false;
    }
}




    void CreatePreviewObject()
    {
        if (currentPreview != null) Destroy(currentPreview);
        
        currentPreview = Instantiate(objectPrefabs[selectedIndex], budenContainer.transform);
        SetMaterialTransparent(currentPreview);
    }

    void PlaceObject()
    {
        GameObject newObj = Instantiate(objectPrefabs[selectedIndex], 
            placementPosition, 
            Quaternion.Euler(0, placementRotationY, 0), 
            budenContainer.transform);

        am.AddBude(newObj.GetComponent<Buden>());
        Destroy(currentPreview);
        selectedIndex = -1;
        hasManualRotation = false;
        hasUsedMoveInput = false;

        // Bewegung im Godmode wieder aktivieren
        if (godmodeController != null && godmodeController.IsGodmodeActive())
        {
            godmodeController.StopPlacingObject();
        }
    }

    void SetMaterialTransparent(GameObject obj)
    {
        foreach(var renderer in obj.GetComponentsInChildren<Renderer>())
        {
            renderer.material = previewMaterial;
        }
    }
}
