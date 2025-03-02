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

    private GameObject currentPreview;
    private int selectedIndex = -1;

    public Transform handTransform;
    public Transform cameraTransform; // VR-Kamera für bessere Positionierung
    
    public InputActionReference moveAction;  // Linker Controller (X/Z-Bewegung) - Vector2
    public InputActionReference rotateAction; // Rechter Controller (Rotation) - float
    public InputActionReference confirmAction;

    private bool isPlacing = false;
    public bool IsPlacing => isPlacing;

    private Vector3 placementPosition;
    private float placementRotationY = 0f;
    private float rotationSpeed = 100f; // Geschwindigkeit der Rotation
    private bool hasManualRotation = false; // Merker für manuelle Drehung

    [SerializeField] private AudioClip placementSoundClip;

    void Start()
    {
        budenContainer = GameObject.Find("BudenContainer");    
        am = GameObject.Find("AgentManager").GetComponent<AgentManager>();
        cameraTransform = Camera.main.transform; // Setzt die VR-Kamera
    }

    public void StartSpawning(int index)
    {
        if(index < 0 || index >= objectPrefabs.Count) return;
        
        isPlacing = true;
        selectedIndex = index;
        CreatePreviewObject();

        // **Startposition: Vor dem Spieler, nicht in der Hand**
        Vector3 forwardOffset = cameraTransform.forward * placementDistance; // Abstand von der Kamera aus
        placementPosition = new Vector3(forwardOffset.x, 0, forwardOffset.z) + cameraTransform.position;
        placementRotationY = cameraTransform.eulerAngles.y; // Initiale Rotation übernimmt Blickrichtung
    }

    void Update()
    {
        if (!isPlacing || currentPreview == null) return;

        // **Falls Trackpad nicht genutzt wird, Handbewegung als Basis nehmen**
        Vector3 handOffset = handTransform.position + handTransform.forward * placementDistance;
        Vector3 newPosition = new Vector3(handOffset.x, 0, handOffset.z); // Y bleibt 0
        
        float handRotationY = handTransform.eulerAngles.y; // Rotation der Hand

        // **Manuelle Steuerung über Trackpad (X/Z-Bewegung)**
        Vector2 moveInput = moveAction.action.ReadValue<Vector2>();  // Linker Controller → Vector2
        float rotationInput = rotateAction.action.ReadValue<float>(); // Rechter Controller → float für Rotation

        // **Falls das Trackpad für Bewegung genutzt wird → Bewege Objekt nur auf X/Z**
        if (moveInput.magnitude > 0.01f) 
        {
            placementPosition += new Vector3(moveInput.x, 0, moveInput.y) * Time.deltaTime * 2f;
        }
        else 
        {
            placementPosition = newPosition; // Falls Trackpad nicht genutzt wird → Bewegung über Hand
        }

        // **Falls Rotationseingabe vorhanden ist → Manuelle Rotation setzen**
        if (Mathf.Abs(rotationInput) > 0.01f) 
        {
            placementRotationY += rotationInput * rotationSpeed * Time.deltaTime;
            hasManualRotation = true;
        }
        else if (!hasManualRotation) 
        {
            placementRotationY = handRotationY; // Falls keine manuelle Rotation gemacht wurde → Handrotation übernehmen
        }

        // **Vorschau-Objekt aktualisieren**
        currentPreview.transform.position = placementPosition;
        currentPreview.transform.rotation = Quaternion.Euler(0, placementRotationY, 0);

        // **Platzierung bestätigen**
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

        //play Placement Sound Effect
        SoundFXManager.instance.PlaySoundFXClip(placementSoundClip, transform, 1f);

        Destroy(currentPreview);
        selectedIndex = -1;
        hasManualRotation = false; // Reset nach Platzierung
    }

    void SetMaterialTransparent(GameObject obj)
    {
        foreach(var renderer in obj.GetComponentsInChildren<Renderer>())
        {
            renderer.material = previewMaterial;
        }
    }
}
