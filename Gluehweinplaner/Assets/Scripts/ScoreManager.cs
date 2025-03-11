using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    [HideInInspector]public TextMeshProUGUI scoreText;
    public Heatmap heatmapScript;
    [SerializeField]private int scoreCount = 0;
    public AgentManager agentManagerScript;

    // Start is called before the first frame update
    void Start()
    {
        scoreText = GetComponent<TextMeshProUGUI>();
        UpdateUI();
    }

    // Update is called once per frame
    void Update()
    {
        Buden[] AlleBuden = agentManagerScript.AlleBuden;
        int BusyBuden = 0;
        foreach (Buden Bude in AlleBuden)
        {
            if (Bude.CheckAuslastung()) BusyBuden++;
        }

        int Buden = agentManagerScript.BudenCount();

        int Agents = agentManagerScript.playerCount;

        //MAX HIER ScoreCount ANPASSEN, DER REST PASSIERT AUTOMATISCH
        //scoreCount = cellScore - BusyBuden * 10;
        UpdateUI();
    }

    private void UpdateUI()
    {
        scoreText.text = "Effizienz Score: " + scoreCount.ToString();
    }
}
