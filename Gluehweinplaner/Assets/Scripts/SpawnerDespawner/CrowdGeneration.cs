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
    public float agentradius = 1f;

    private float zeitVergangen;

    private AgentManager am;
    private MeshCollider col;
    private List<Vector3> m_agentPositions;

    // Start is called before the first frame update
    void Start()
    {
        zeitVergangen = spawnTime;
        am = GameObject.Find("AgentManager").GetComponent<AgentManager>();

        prop = Resources.Load("agent") as GameObject;
        col = GetComponent<MeshCollider>();
        // Initialize my agents' position list
        m_agentPositions = new List<Vector3>();

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
                Vector3 position = GenerateRandomPosition();
                Quaternion rotation = Quaternion.Euler(0, 0, 0);

                // Instantiate agent
                GameObject agent = Instantiate(prop, position, rotation);

                // Set the parent of the instantiated props to be this CrowdGenerator
                agent.transform.parent = transform;

                m_agentPositions.Add(position);
                zeitVergangen = spawnTime;
            }
        }
    }


    public Vector3 GenerateRandomPosition()
    {
        Vector3 position;
        do
        {
            float cellX = Random.Range(minWorldLimitX, maxWorldLimitX);
            float cellZ = Random.Range(minWorldLimitZ, maxWorldLimitZ);
            position = new Vector3(cellX, col.bounds.min.y + 1, cellZ);


        } while (Physics.CheckSphere(position,agentradius));

        return position;
    }

}