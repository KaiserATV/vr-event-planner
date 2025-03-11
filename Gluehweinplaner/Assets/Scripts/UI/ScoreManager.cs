using UnityEngine;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    public TextMeshProUGUI scoreText;
    public Heatmap heatmapScript;
    [SerializeField]private float scoreCount = 0;
    public AgentManager agentManagerScript;
    private bool show=;

   public void ToggleScore()
{
    Debug.Log("Score wird berechnet...");

    Buden[] AlleBuden = agentManagerScript.alleBuden;
    int BusyBuden = 0;
    
    foreach (Buden Bude in AlleBuden)
    {
        if (Bude.CheckAuslastung()) BusyBuden++;
    }

    int effectiveBusyBuden = (BusyBuden == 0) ? 1 : BusyBuden; // Falls 0, dann 1 nehmen

    scoreCount = CalcHeatMapScore() + effectiveBusyBuden * (agentManagerScript.playerCount / AlleBuden.Length);

    Debug.Log("Score: " + scoreCount);
    Debug.Log("Heatmap: " + CalcHeatMapScore());
    Debug.Log("BusyBuden: " + BusyBuden);
    Debug.Log("Effective BusyBuden: " + effectiveBusyBuden);
    Debug.Log("PlayerCount: " + agentManagerScript.playerCount);
    Debug.Log("BudenCount: " + AlleBuden.Length);

    UpdateUI();
}

        
            // Start is called before the first frame update
    void Start()
    {
        UpdateUI();
    }

    private float CalcHeatMapScore()
{
    int[] array = heatmapScript.playMaxCount;

    if (array == null || array.Length == 0)
    {
        return 0;
    }

    int good = 0;
    int bad = 0;

    for (int i = 0; i < array.Length; i++)
    {
        if (array[i] <= usageCat.medium)
        {
            good++;
        }
        else
        {
            bad++;
        }
    }

    Debug.Log($"Final good count: {good}, bad count: {bad}");

    if (good == 0) return 0; // Vermeidung von Division durch Null

    return ((float)bad / good * 2000);
}


    private void UpdateUI()
    {
        if (show)
        {
            scoreText.text = "Effizienz Score: " + scoreCount.ToString();
        }
        else
        {
            scoreText.text = "";
        }
    }
}
