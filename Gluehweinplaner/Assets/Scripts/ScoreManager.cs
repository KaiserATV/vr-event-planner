using UnityEngine;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    public TextMeshProUGUI scoreText;
    public Heatmap heatmapScript;
    [SerializeField]private float scoreCount = 0;
    public AgentManager agentManagerScript;
    private bool show=false;

    public void ToogleScore()
    {
        show = !show;
    }

    // Start is called before the first frame update
    void Start()
    {
        UpdateUI();
    }

    // Update is called once per frame
    void Update()
    {
        if (show)
        {
            Buden[] AlleBuden = agentManagerScript.AlleBuden;
            int BusyBuden = 0;
            foreach (Buden Bude in AlleBuden)
            {
                if (Bude.CheckAuslastung()) BusyBuden++;
            }
            scoreCount = CalcHeatMapScore() + BusyBuden * (agentManagerScript.playerCount / AlleBuden.Length);
            UpdateUI();
        }
    }

    private float CalcHeatMapScore()
    {
        int[] array = heatmapScript.playMaxCount;
        int good = 0;
        int bad = 0;
        foreach (int i in array)
        {
            if(i > usageCat.medium)
            {
                bad++;
            }
            else
            {
                good++;
            }
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
