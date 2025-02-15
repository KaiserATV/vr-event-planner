using System.Collections.Generic;
using UnityEngine;

public class CrowdGeneration : MonoBehaviour
{
    private GameObject prop;

    public float minWorldLimitX = 0;
    public float maxWorldLimitX = 0;
    public float minWorldLimitZ = 0;
    public float maxWorldLimitZ = 0;
    public float spawnTime = 1f;

    private float zeitVergangen;


    private AgentManager am;
    private MeshCollider col;
    private List<Vector3> m_agentPositions;
    private float m_minDistance;
    private HashSet<Vector3> obstaclePositions;

    // Start is called before the first frame update
    void Start()
    {
        zeitVergangen = spawnTime;
        am = GameObject.Find("AgentManager").GetComponent<AgentManager>();

        prop = Resources.Load("agent") as GameObject;
        col = GetComponent<MeshCollider>();
        // Initialize my agents' position list
        m_agentPositions = new List<Vector3>();

        // Initialize obstaclePositions as an empty HashSet
        obstaclePositions = new HashSet<Vector3>();

        // Define the minimum possible instantiation distance
        m_minDistance = prop.transform.localScale.y;

        minWorldLimitX = col.bounds.min.x;
        maxWorldLimitX = col.bounds.max.x;
        minWorldLimitZ = col.bounds.min.z;
        maxWorldLimitZ = col.bounds.max.z;
    }

    private void FixedUpdate()
    {
        if (am.simulating)
        {
            zeitVergangen -= Time.deltaTime;
            if (zeitVergangen > 0)
            {

            }
            else if (am.CanAddPlayer())
            {
                Vector3 position = GenerateRandomPosition(m_agentPositions);
                Quaternion rotation = Quaternion.Euler(0, 0, 0);

                // Instantiate agent
                GameObject agent = Instantiate(prop, position, rotation);

                // Set the parent of the instantiated props to be this CrowdGenerator
                agent.transform.parent = transform;

                m_agentPositions.Add(position);
                am.addPlayer();
                zeitVergangen = spawnTime;
            }
        }
    }


    private Vector3 GenerateRandomPosition(List<Vector3> takenPositions)
    {
        Vector3 position;
        do
        {
            float cellX = Random.Range(minWorldLimitX, maxWorldLimitX);
            float cellZ = Random.Range(minWorldLimitZ, maxWorldLimitZ);
            position = new Vector3(cellX, col.bounds.min.y + 1, cellZ);


        } while (IsPositionTooClose(position, takenPositions) || obstaclePositions.Contains(position));

        return position;
    }

    // Check if the position is too close to any existing positions
    bool IsPositionTooClose(Vector3 position, List<Vector3> existingPositions)
    {
        foreach (Vector3 existingPosition in existingPositions)
        {
            if (Vector3.Distance(position, existingPosition) < m_minDistance)
            {
                return true; // Too close, generate a new position
            }
        }
        return false; // Position is fine
    }

}