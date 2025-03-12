using UnityEngine;
using TMPro;
using System;

public class ScoreManager : MonoBehaviour
{
    public TextMeshProUGUI scoreText;
    public Heatmap heatmapScript;
    [SerializeField]private float scoreCount = 0;
    public AgentManager agentManagerScript;
    private bool show=true;

   public void ToggleScore()
{

    Buden[] AlleBuden = agentManagerScript.alleBuden;
    int BusyBuden = 0;
    
    foreach (Buden Bude in AlleBuden)
    {
        if (Bude.CheckAuslastung()) BusyBuden++;
    }

    int effectiveBusyBuden = (BusyBuden == 0) ? 1 : BusyBuden; // Falls 0, dann 1 nehmen

    scoreCount = CalcHeatMapScore() + effectiveBusyBuden/AlleBuden.Length + agentManagerScript.agentsLostPatience/agentManagerScript.maxPlayerCount + Math.Abs(agentManagerScript.maxPlayerCount-agentManagerScript.maxKapazitaet);
    scoreCount = (float) Math.Round(scoreCount, 2); 
    UpdateUI();
}

   
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
