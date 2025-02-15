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
    public InputActionReference moveAction;  // Bewegung X/Z
    public InputActionReference rotateAction; // Rotation um Y
    public InputActionReference confirmAction;

    private bool isPlacing = false;
    public bool IsPlacing => isPlacing;

    private Vector3 placementPosition;
    private float placementRotationY = 0f;
    private float rotationSpeed = 100f; // Geschwindigkeit der Rotation

    void Start()
    {
        budenContainer = GameObject.Find("BudenContainer");    
        am = GameObject.Find("AgentManager").GetComponent<AgentManager>();
    }

    public void StartSpawning(int index)
    {
        if(index < 0 || index >= objectPrefabs.Count) return;
        
        isPlacing = true;
        selectedIndex = index;
        CreatePreviewObject();

        // Startposition etwas vor der Hand
        Vector3 forwardOffset = handTransform.forward * placementDistance;
        placementPosition = new Vector3(forwardOffset.x, 0, forwardOffset.z);
    }

    void Update()
    {
        if (!isPlacing || currentPreview == null) return;

        // Controller-Eingaben auslesen
        Vector2 moveInput = moveAction.action.ReadValue<Vector2>(); 
        float rotationInput = rotateAction.action.ReadValue<float>(); 

        // Position auf X/Z anpassen
        placementPosition += new Vector3(moveInput.x, 0, moveInput.y) * Time.deltaTime * 2f; 

        // Rotation um die Y-Achse steuern
        placementRotationY += rotationInput * rotationSpeed * Time.deltaTime;

        // Vorschau-Objekt aktualisieren
        currentPreview.transform.position = placementPosition;
        currentPreview.transform.rotation = Quaternion.Euler(0, placementRotationY, 0);

        // Platzierung bestätigen
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
    }

    void SetMaterialTransparent(GameObject obj)
    {
        foreach(var renderer in obj.GetComponentsInChildren<Renderer>())
        {
            renderer.material = previewMaterial;
        }
    }
}
