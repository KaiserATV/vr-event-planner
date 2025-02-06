using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System;
using System.Linq;
using Valve.VR; // SteamVR Input

public class RadicalSelection : MonoBehaviour
{
    [Range(2, 10)]
    public int numberOfRadialPart;
    public GameObject radialPartPrefab;
    public Transform radialPartCanvas;
    public float angleBetweenPart = 10;
    public Transform handTransform;
    public ObjectSpawner objectSpawner;

    public UnityEvent<int> OnPartSelected;
    private List<GameObject> spawnedParts = new List<GameObject>();
    private int currentSelectedRadialPart = -1;

    public SteamVR_Action_Boolean menuActivateAction; // SteamVR Input Action

    void Start()
    {
        menuActivateAction = SteamVR_Actions.default_RadialMenu; // Falls deine Action anders heißt, ersetze sie hier
    }

    void Update()
    {
        Debug.Log("SteamVR Action: " + menuActivateAction.GetState(SteamVR_Input_Sources.Any));

        // Check if the menu activation button is pressed
        if (menuActivateAction.GetStateDown(SteamVR_Input_Sources.Any) && !objectSpawner.IsPlacing)
        {
            radialPartCanvas.gameObject.SetActive(true); // Show the radial menu
            SpawnRadialPart(); // Populate the radial menu
        }

        // Keep the menu active and update selection
        if (radialPartCanvas.gameObject.activeSelf)
        {
            GetSelectedRadialPart();

            // Hide and trigger the selected part when the button is released
            if (menuActivateAction.GetStateUp(SteamVR_Input_Sources.Any))
            {
                HideAndTriggerSelected();
            }
        }
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
