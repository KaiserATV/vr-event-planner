using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AgentController : MonoBehaviour
{
    // Start is called before the first frame update
    private NavMeshAgent agent;
    public int goalsBeforeExit = 0;
    public bool exiting=false;
    public bool waiting = false;
    public float timeLeftWaiting = 0.0f;


    public int goalNr;



    private AgentManager sm;
    private List<int> visitedGoalNumbers =  new List<int>();

    public Vector3Int cells;
    private Vector3 goal;


    // Update is called once per frame
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.autoRepath = false;
        sm = GameObject.Find("AgentManager").GetComponent<AgentManager>();
        FindNextGoal();
        agent.destination = new Vector3(goal.x, 0, goal.y);
    }

    void Update()
    {
        if (waiting)
        {
            timeLeftWaiting -= Time.deltaTime;
            if(timeLeftWaiting < 0) { sm.DeRegisterPlayer(this, cells, goalNr) ;waiting = false; FindNextGoal();}
        }
        else if (agent.remainingDistance == 0 && !exiting)
        {
            timeLeftWaiting = sm.GetWaitTime(goalNr);
            waiting = true;
        }else if (agent.remainingDistance == 0 && exiting)
        {
            Destroy(this.gameObject);
        }
    }


    void FindNextGoal()
    {
        if (goalsBeforeExit > 0 && !exiting)
        {
            do
            {
                cells = sm.GetNewCellsPos(out goalNr, this);
                if (goalNr == -1) { FindExit(); return; }
            } while (visitedGoalNumbers.Contains(goalNr));
            visitedGoalNumbers.Add(goalNr);
            goal = sm.GetNewWorldPos(cells, goalNr);
            goalsBeforeExit--;
        }
        else
        {
            FindExit();
        }
    }

    void FindExit()
    {
        exiting = true;
        goal = sm.GetClostestExit(transform.position);
        agent.destination = new Vector3(goal.x, 0, goal.y);
    }

    public void InvalidatePosition(Vector3 newCoords)
    {
        if (waiting) { waiting = false; }
        goal = newCoords;
        agent.destination = new Vector3(goal.x, 0, goal.y);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(agent.destination, radius: 0.2f);
    }


}
