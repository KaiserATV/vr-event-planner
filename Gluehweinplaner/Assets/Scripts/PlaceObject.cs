using UnityEngine;
using Valve.VR;

public class PrefabPlacer : MonoBehaviour
{
    public SteamVR_Input_Sources handType = SteamVR_Input_Sources.RightHand;
    public SteamVR_Action_Boolean placeAction = SteamVR_Actions.default_GrabPinch;

    private GameObject selectedPrefab;

    void Update()
    {
        if (placeAction.GetStateDown(handType) && selectedPrefab != null)
        {
            Ray ray = new Ray(transform.position, transform.forward);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, 10f))
            {
                Instantiate(selectedPrefab, hit.point, Quaternion.identity);
                Debug.Log("Prefab platziert an: " + hit.point);
            }
        }
    }

    public void SetSelectedPrefab(GameObject prefab)
    {
        selectedPrefab = prefab;
    }
}
