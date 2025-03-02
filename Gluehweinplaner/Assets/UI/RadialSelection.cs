using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System;
using UnityEngine.InputSystem;
using System.Linq;
using Unity.VisualScripting;
using TMPro;


public class RadicalSelection : MonoBehaviour
{
    [Range(2, 10)]
    public int numberOfRadialPart;
    public GameObject radialPartPrefab;
    public Transform radialPartCanvas;
    public float angleBetweenPart = 10;
    public Transform handTransform;
    public ObjectSpawner objectSpawner;
    public List<UnityEvent<int>> partToFunction;
    public Gradient gradient = new Gradient();

    private List<GameObject> spawnedParts = new List<GameObject>();
    private int currentSelectedRadialPart = -1;

    public List<string> buttonLabels;  // Text for each button
    public float textRotationOffset = 90f;  // Keep text upright

    public InputActionReference menuActivateAction;

    public float waitTimeUntilActivation = 1.0f;
    public float timeWaited = 0;
    public int waitingAt=-1;

    [SerializeField] private AudioClip spawnRadialPartSoundClip;
    [SerializeField] private AudioClip selectionChangeSoundClip;
    private int previousSelected = -1; // Track previous selection



    void Start()
    {
        buttonLabels.Add("Veranstaltungsobjekt");
        buttonLabels.Add("Besucherstrom");

        //Debug.Log($"RadialPartCanvas Active: {radialPartCanvas.gameObject.activeSelf}");
        //Debug.Log($"Hand Position: {handTransform.position}, Rotation: {handTransform.rotation}");
        //Debug.Log($"Current Selected Part: {currentSelectedRadialPart}");


        var colors = new GradientColorKey[2];
        colors[0] = new GradientColorKey(Color.white, 0.0f);
        colors[1] = new GradientColorKey(Color.yellow, 1.0f);

        var alphas = new GradientAlphaKey[2];
        alphas[0] = new GradientAlphaKey(1.0f, 0.0f);
        alphas[1] = new GradientAlphaKey(1.0f, 1.0f);

        gradient.SetKeys(colors, alphas);

    }

    void Update()
    {
        //Debug.Log($"RadialPartCanvas Active: {radialPartCanvas.gameObject.activeSelf}");
        //Debug.Log($"Hand Position: {handTransform.position}, Rotation: {handTransform.rotation}");
        //Debug.Log($"Current Selected Part: {currentSelectedRadialPart}");
        
        // Check if the menu activation button is pressed
        if (menuActivateAction.action.triggered && !objectSpawner.IsPlacing)
        {
            radialPartCanvas.gameObject.SetActive(true); // Show the radial menu
            SpawnRadialPart(); // Populate the radial menu
        }

        // Keep the menu active and update selection
        if (radialPartCanvas.gameObject.activeSelf)
        {
            GetSelectedRadialPart(Time.deltaTime);

            // Hide and trigger the selected part when the button is released
            if (menuActivateAction.action.WasReleasedThisFrame())
            {
                HideAndTriggerSelected();
            }
        }

    }

    private void ResetWaitTimer(int newWait)
    {
        timeWaited = 0;
        waitingAt = newWait;
    }

    private void HideAndTriggerSelected()
    {
        if(timeWaited > waitTimeUntilActivation && currentSelectedRadialPart < partToFunction.Count)
        {
            partToFunction[currentSelectedRadialPart].Invoke(currentSelectedRadialPart);
        }
            radialPartCanvas.gameObject.SetActive(false);
    }

    public void GetSelectedRadialPart(float time)
    {
        Vector3 centerToHand = handTransform.position - radialPartCanvas.position;
        Vector3 centerToHandProjected = Vector3.ProjectOnPlane(centerToHand, radialPartCanvas.forward);

        float angle = Vector3.SignedAngle(radialPartCanvas.up, centerToHandProjected, -radialPartCanvas.forward);

        if (angle < 0)
        {
            angle += 360;
        }

        //Debug.Log("ANGLE: " + angle);

        currentSelectedRadialPart = (int)angle * numberOfRadialPart / 360;

        // Play sound when selection changes
        if (previousSelected != currentSelectedRadialPart)
        {
            SoundFXManager.instance.PlaySoundFXClip(selectionChangeSoundClip, transform, 0.8f);
            previousSelected = currentSelectedRadialPart;
        }

        if(currentSelectedRadialPart != waitingAt)
        {
            ResetWaitTimer(currentSelectedRadialPart);
        }
        timeWaited += time;

        for (int i = 0; i < spawnedParts.Count; i++)
        {
            if (i == currentSelectedRadialPart)
            {
                spawnedParts[i].GetComponent<Image>().color = gradient.Evaluate(timeWaited/waitTimeUntilActivation);
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

        //play Placement Sound Effect
        SoundFXManager.instance.PlaySoundFXClip(spawnRadialPartSoundClip, transform, 1f);

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

            // Add icon setup
            //var iconImage = spawnedRadialPart.transform.GetChild(0).GetComponent<Image>();
            //iconImage.sprite = buttonIcons[i];
            //iconImage.rectTransform.sizeDelta = new Vector2(iconSize, iconSize);
            TextMeshProUGUI buttonText = spawnedRadialPart.GetComponentInChildren<TextMeshProUGUI>();
            if (buttonText != null && i < buttonLabels.Count)
            {
                buttonText.text = buttonLabels[i];
                // Counteract radial rotation
            }

            //// Add icon implementation (optional)
            //Image iconImage = spawnedRadialPart.GetComponentInChildren<Image>();
            //if (iconImage != null && i < buttonIcons.Count)
            //{
            //    iconImage.sprite = buttonIcons[i];
            //    iconImage.rectTransform.sizeDelta = new Vector2(iconSize, iconSize);
            //}
        }
    }
}

