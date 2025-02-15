using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System;
using UnityEngine.InputSystem;
using System.Linq;

public class ObjectSpawner : MonoBehaviour
{
    public List<GameObject> objectPrefabs; // Spawnable prefabs
    public float placementDistance = 2f; // Distance from hand
    public Material previewMaterial; // Transparent material for preview
    public GameObject budenContainer;
    public AgentManager am;

    private GameObject currentPreview;
    private int selectedIndex = -1;

    public Transform handTransform; 
    public InputActionReference menuActivateAction; 

    public InputActionReference confirmAction;
    private bool isPlacing = false;
    public bool IsPlacing => isPlacing;

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
    }

    void Update()
    {
        if (currentPreview != null)
        {
            currentPreview.transform.position = handTransform.position + handTransform.forward * placementDistance;
            currentPreview.transform.rotation = handTransform.rotation;

            // Handle placement confirmation
            if (confirmAction.action.triggered)
            {
                PlaceObject();
                isPlacing = false;
            }
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
        // Instantiate final object
        Instantiate(objectPrefabs[selectedIndex], 
                  currentPreview.transform.position, 
                  currentPreview.transform.rotation, budenContainer.transform);
        am.AddBude(objectPrefabs[selectedIndex].GetComponent<Buden>());
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