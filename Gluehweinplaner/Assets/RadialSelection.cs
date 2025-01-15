using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System;


public class RadicalSelection : MonoBehaviour
{
    //public OVRInput.Button spawnButton;
    [Range(2, 10)]
    public int numberOfRadialPart;
    public GameObject radialPartPrefab;
    public Transform radialPartCanvas;
    public float angleBetweenPart = 10;
    public Transform handTransform;

    public UnityEvent<int> OnPartSelected;

    private List<GameObject> spawnedParts = new List<GameObject>();
    private int currentSelectedRadialPart = -1;


    void Start()
    {
        Debug.Log($"RadialPartCanvas Active: {radialPartCanvas.gameObject.activeSelf}");
        Debug.Log($"Hand Position: {handTransform.position}, Rotation: {handTransform.rotation}");
        Debug.Log($"Current Selected Part: {currentSelectedRadialPart}");

    }

    void Update()
    {
        /*if(OVRInput.GetDown(spawnButton))
        {
            SpawnRadialPart();
        }
        if(OVRInput.Get(spawnButton))
        {
            GetSelectedRadialPart();
        }
        if(OVRInput.GetUp(spawnButton))
        {
            HideAndTriggerSelected();
        }*/
        SpawnRadialPart();
        GetSelectedRadialPart();
        HideAndTriggerSelected();
        radialPartCanvas.gameObject.SetActive(true); // Ensure canvas is active for testing
        Debug.Log($"RadialPartCanvas Active: {radialPartCanvas.gameObject.activeSelf}");
        Debug.Log($"Hand Position: {handTransform.position}, Rotation: {handTransform.rotation}");
        Debug.Log($"Current Selected Part: {currentSelectedRadialPart}");

    }

    private void HideAndTriggerSelected()
    {
        OnPartSelected.Invoke(currentSelectedRadialPart);
        radialPartCanvas.gameObject.SetActive(false);
    }

    public void GetSelectedRadialPart()
    {
        Vector3 centerToHand = handTransform.position - radialPartCanvas.position;
        Vector3 centerToHandProjected = Vector3.ProjectOnPlane(centerToHand, radialPartCanvas.forward);

        float angle = Vector3.SignedAngle(radialPartCanvas.up, centerToHandProjected, -radialPartCanvas.forward);

        if (angle < 0)
        {
            angle += 360;
        }

        Debug.Log("ANGLE" + angle);

        currentSelectedRadialPart = (int)angle * numberOfRadialPart / 360;

        for (int i = 0; i < spawnedParts.Count; i++)
        {
            if (i == currentSelectedRadialPart)
            {
                spawnedParts[i].GetComponent<Image>().color = Color.yellow;
                spawnedParts[i].transform.localScale = 1.1f * UnityEngine.Vector3.one;
            }
            else
            {
                spawnedParts[i].GetComponent<Image>().color = Color.white;
                spawnedParts[i].transform.localScale = 1 * UnityEngine.Vector3.one;
            }
        }
    }

    public void SpawnRadialPart()
    {
        radialPartCanvas.gameObject.SetActive(true);
        radialPartCanvas.position = handTransform.position;
        radialPartCanvas.rotation = handTransform.rotation;

        foreach (var item in spawnedParts)
        {
            Destroy(item);
        }

        spawnedParts.Clear();

        for (int i = 0; i < numberOfRadialPart; i++)
        {
            float angle = -i * 360 / numberOfRadialPart - angleBetweenPart / 2;
            Vector3 radialPartEulerAngle = new Vector3(0, 0, angle);

            GameObject spawnedRadialPart = Instantiate(radialPartPrefab, radialPartCanvas);
            spawnedRadialPart.transform.position = radialPartCanvas.position;
            spawnedRadialPart.transform.localEulerAngles = radialPartEulerAngle;

            spawnedRadialPart.GetComponent<Image>().fillAmount = (1 / (float)numberOfRadialPart) - (angleBetweenPart / 360);
            spawnedParts.Add(spawnedRadialPart);
        }
    }

}

