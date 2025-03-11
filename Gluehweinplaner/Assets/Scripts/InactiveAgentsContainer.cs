using System.Collections.Generic;
using UnityEngine;

public class InactiveAgentsContainer : MonoBehaviour
{
    private Vector3 WorldCoords;
    private int StoredPlayerCount;
    private LinkedList<AgentController> StoredAgents;
    private void Start()
    {
        WorldCoords = this.transform.position;
        StoredPlayerCount = 0;
    }

    public void AddAgent(AgentController ac)
    {
        StoredAgents.AddFirst(ac);
        StoredPlayerCount++;
    }
    public AgentController GetAgent()
    {
        AgentController ac = null;
        if (StoredPlayerCount != 0)
        {
            ac = StoredAgents.First.Value;
            StoredPlayerCount--;
            StoredAgents.RemoveFirst();
        }
        return ac;
    }

    public Vector3 GetWorldCoords() { return WorldCoords; }
    public int GetStoredCount() { return StoredPlayerCount; }

}
