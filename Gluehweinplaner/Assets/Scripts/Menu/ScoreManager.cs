using UnityEngine;
using TMPro;
using System;

public class ScoreManager : MonoBehaviour
{
    public TextMeshProUGUI scoreText;
    public Heatmap heatmapScript;
    [SerializeField]private float scoreCount = 0;
    public AgentManager agentManagerScript;

   public void UpdateScore()
{

    Buden[] AlleBuden = agentManagerScript.alleBuden;
    int BusyBuden = 0;
    
    foreach (Buden Bude in AlleBuden)
    {
            if(Bude != null)
            {
                if (Bude.CheckAuslastung()) BusyBuden++;
            }
        }

    int effectiveBusyBuden = (BusyBuden == 0) ? 1 : BusyBuden; // Falls 0, dann 1 nehmen

        int countKapa = agentManagerScript.maxPlayerCount - agentManagerScript.maxKapazitaet;
        countKapa *= -1;
        if (countKapa < 0) {
            countKapa = 0;
        }

    scoreCount = CalcHeatMapScore()*33 + (effectiveBusyBuden/AlleBuden.Length)*33 + (agentManagerScript.agentsLostPatience/agentManagerScript.maxPlayerCount)*33 + countKapa;
    scoreCount = (float)Math.Round(scoreCount, 2);

    scoreText.text = scoreCount.ToString();
}

   
    void Start()
    {
        this.gameObject.SetActive(false);
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
            int playerCount = array[i];
        if (playerCount <= usageCat.medium && playerCount > 0)
        {
            good++;
        }
        else
        {
            bad++;
        }
    }

    if (good == 0) return 0; // Vermeidung von Division durch Null

    return (bad / good);
}

    public void ToggleEffizenzScore()
    {
        this.gameObject.SetActive(!this.gameObject.activeInHierarchy);
        if(this.gameObject.activeInHierarchy == true)
        {
            Transform referenceTransform = Camera.main.transform;
            float distance = 1.75f;

            Vector3 forwardDirection = Vector3.ProjectOnPlane(referenceTransform.forward, Vector3.up).normalized;
            this.transform.position = referenceTransform.position + forwardDirection * distance;

            Quaternion lookRotation = Quaternion.LookRotation(forwardDirection, Vector3.up);
            this.transform.rotation = lookRotation;
            UpdateScore();
        }
    }

}
