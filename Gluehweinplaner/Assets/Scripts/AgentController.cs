using System.Collections.Generic;
using System.ComponentModel;
using Unity.VisualScripting;
using Unity.XR.CoreUtils;
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

    private Vector2Int bitarrayCells;
    private Vector2Int positionCells;
    private Vector2 goal;
    private BitArray2D bude;
    private AgentManager sm;
    private List<int> visitedGoalNumbers =  new List<int>();


    public const float updateRate = 5.0f;
    private float timeLeft = updateRate;


    // Update is called once per frame
    void Start()
    {
        sm = GameObject.Find("AgentManager").GetComponent<AgentManager>();
        agent = GetComponent<NavMeshAgent>();
        agent.autoRepath = true;
        sm.addPlayer(this);
        positionCells = sm.UpdatePositionInGrid(new Vector2(transform.position.x, transform.position.z));
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
                if (timeLeftWaiting < 0 && bude != null) { 
                    bude.RemovePlayer(bitarrayCells, this); 
                    waiting = false; 
                    FindNextGoal(); }
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
            else
            {
                //if (timeLeft > 0)
                //{
                //    timeLeft -= Time.deltaTime;
                //}
                //else
                //{
                    positionCells = sm.UpdatePositionInGrid(positionCells, new Vector2(transform.position.x, transform.position.z));
                    //timeLeft = updateRate;
                //}

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
        exiting = false;

        positionCells = sm.UpdatePositionInGrid(positionCells,new Vector2(transform.position.x, transform.position.z));

        timeLeftWaiting = 0.0f;
        visitedGoalNumbers = new List<int>();
        bude = null;
        
        FindNextGoal();

        if (randomExitGoalNumber) { goalsBeforeExit = Random.Range(0, sm.BudenCount() + 1); }
        
        agent.destination = new Vector3(goal.x, 0, goal.y);
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
        bitarrayCells = v;
    }
    public Vector2Int GetCells()
    {
        return bitarrayCells;
    }
}
