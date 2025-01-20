using UnityEngine;

public class PrefabSelector : MonoBehaviour
{
    public GameObject[] prefabs; // Liste der Prefabs
    private GameObject selectedPrefab; // Aktuell ausgewähltes Prefab

    public void SelectPrefab(int index)
    {
        if (index >= 0 && index < prefabs.Length)
        {
            selectedPrefab = prefabs[index];
            Debug.Log("Ausgewähltes Prefab: " + selectedPrefab.name);
        }
    }

    public GameObject GetSelectedPrefab()
    {
        return selectedPrefab;
    }
}
