using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

public class AgentController : MonoBehaviour
{
    // Start is called before the first frame update
    public Camera cam;
    public NavMeshAgent agent;

    private AgentManager sm;

    public int goalNr;

    private Vector3Int cells;
    private Vector3 goal;


    // Update is called once per frame
    void Start()
    {
        sm = GameObject.Find("AgentManager").GetComponent<AgentManager>();
        cells = sm.GetNewCellsPos(out goalNr);
        goal = sm.GetNewWorldPos(cells, goalNr);
        agent.destination = new Vector3(goal.x,0,goal.y);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(agent.destination, radius: 0.2f);
    }


}
