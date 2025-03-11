using UnityEngine;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    public TextMeshProUGUI scoreText;
    public Heatmap heatmapScript;
    [SerializeField]private float scoreCount = 0;
    public AgentManager agentManagerScript;
    private bool show=true;

    public void ToggleScore()
    {
        Debug.Log("Toggle Score");
        show = !show;
        if (show)
        {
            Buden[] AlleBuden = agentManagerScript.alleBuden;
            int BusyBuden = 0;
            foreach (Buden Bude in AlleBuden)
            {
                if (Bude.CheckAuslastung()) BusyBuden++;
            }
            scoreCount = CalcHeatMapScore() + BusyBuden * (agentManagerScript.playerCount / AlleBuden.Length);
            // Debug.Log("Score: " + scoreCount);
            // Debug.Log("Heatmap: " + CalcHeatMapScore());
            // Debug.Log("BusyBuden: " + BusyBuden);
            // Debug.Log("PlayerCount: " + agentManagerScript.playerCount);
            // Debug.Log("BudenCount: " + AlleBuden.Length);
            UpdateUI();
        }  
        }
        
            // Start is called before the first frame update
    void Start()
    {
        UpdateUI();
    }

    private float CalcHeatMapScore()
    {
        int[] array = heatmapScript.playMaxCount;
        int good = 0;
        int bad = 0;
        for(int i=0;i < array.Length; i++)
        {
            Debug.Log(i);
            Debug.Log("usageCat.medium" + usageCat.medium);
            if (array[i] <= 6)
            {
                good++;
            }
            else
            {
                bad++;
            }
        Debug.Log("bad" + bad);
        Debug.Log("good" + good);
        }
        return (bad/good* 2000);

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
