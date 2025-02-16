using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class AgentController : MonoBehaviour
{
    // Start is called before the first frame update
    private NavMeshAgent agent;
    
    public bool randomExitGoalNumber = true;
    public int goalsBeforeExit;
    public int goalNr;


    public bool stopped = false;
    public bool waiting = false;
    public float timeLeftWaiting = 0.0f;


    public bool exiting = false;
    public float goalThreshhold = 0.1f;
    public float exitTrashhold = 1f;

    private Vector2Int cells;
    private Vector2 goal;
    private BitArray2D bude;
    private AgentManager sm;
    private List<int> visitedGoalNumbers =  new List<int>();



    // Update is called once per frame
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.autoRepath = true;
        sm = GameObject.Find("AgentManager").GetComponent<AgentManager>();
        sm.addPlayer(this);
        if (randomExitGoalNumber) { goalsBeforeExit = Random.Range(0, sm.BudenCount() + 1); }
        FindNextGoal();
        agent.destination = new Vector3(goal.x, 0, goal.y);
    }

    void Update()
    {
        if (!stopped)
        {
            if (waiting)
            {
                timeLeftWaiting -= Time.deltaTime;
                if (timeLeftWaiting < 0) { bude.RemovePlayer(cells,this); waiting = false; FindNextGoal(); }
            }
            else if (agent.remainingDistance < goalThreshhold && !exiting)
            {
                timeLeftWaiting = sm.GetWaitTime(goalNr);
                waiting = true;
                agent.isStopped = true;
            }
            else if (agent.remainingDistance < exitTrashhold && exiting)
            {
                Destroy();
            }
        }
    }


    void FindNextGoal()
    {
        timeLeftWaiting = 0.0f;
        if (goalsBeforeExit > 0 && !exiting)
        {
            do
            {
                goalNr = sm.GetNewCoords(this, visitedGoalNumbers);
                if (goalNr == -1) { FindExit(); return; }
            } while (visitedGoalNumbers.Contains(goalNr));
            visitedGoalNumbers.Add(goalNr);
            goalsBeforeExit--;
        }
        else
        {
            FindExit();
        }
        agent.isStopped = false;
    }

    void FindExit()
    {
        exiting = true;
        goal = sm.GetClostestExit(transform.position);
        agent.destination = new Vector3(goal.x, 0, goal.y);
    }

    public void InvalidatePosition(Vector3 newCoords)
    {
        if (waiting) { waiting = false; agent.isStopped = false; }
        goal = newCoords;
        agent.destination = new Vector3(goal.x, 0, goal.y);
    }

    public void Destroy()
    {
        agent.Warp(sm.GetNewSpawnPoint());//to random spawner
        stopped = false;
        waiting = false;
        timeLeftWaiting = 0.0f;
        visitedGoalNumbers = new List<int>();
        goal = new Vector2(-1,-1);
        exiting = false;
        cells = new Vector2Int(-1, -1);
}

    public void Stop()
    {
        agent.isStopped = true;
        stopped = true;
    }

    public void Resume()
    {
        agent.isStopped = false;
        stopped = false;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        if (!waiting)
        {
        Gizmos.DrawSphere(agent.destination, radius: 0.2f);
        }
        else
        {
        Gizmos.DrawSphere(agent.nextPosition, radius: 0.2f);
        }
    }

    public void SetGoal(Vector2 g)
    {
        goal = g;
    }
    public void SetBude(BitArray2D b)
    {
        bude = b;
    }
    public void SetCells(Vector2Int v)
    {
        cells = v;
    }
    public Vector2Int GetCells()
    {
        return cells;
    }
}
