using JetBrains.Annotations;
using UnityEngine;

public class Buden : MonoBehaviour
{
    public float agentRadius = 1f;

    public float waitTime = 10.0f;

    private bool komplettAusgelastet = false;

    //To-Do: Optimize the Datastructure
    private BitArray2D wait_B;
    private BitArray2D wait_L;
    private BitArray2D wait_R;
    private BitArray2D ziel;

    // Start is called before the first frame update
    public void Start()
    {
        this.transform.hasChanged = false;
        
        // !!!!IMPORTANT!!!! the number of the children specifies the position in the prefab, if changed, chang number here!!!!!!!
        // 1 - Wait_B, 2 - Wait_L, 3 - Wait_R, 4 - Ziel
        //0 -directly infront of Bode, 1 - to the left of the Bude, 2- to the right of the Bude
        //ziel Array
        Transform child = this.transform.GetChild(4);
        Bounds bound = child.GetComponent<MeshRenderer>().bounds;
        ziel = new BitArray2D(bound, child, agentRadius, 0);

        //Wait_B Array
        child = this.transform.GetChild(1);
        bound = child.GetComponent<MeshRenderer>().bounds;
        wait_B = new BitArray2D(bound, child, agentRadius, 0);

        //Wait_L Array
        child = this.transform.GetChild(2);
        bound = child.GetComponent<MeshRenderer>().bounds;
        wait_L = new BitArray2D(bound, child, agentRadius, 1);

        //Wait_R Array
        child = this.transform.GetChild(3);
        bound = child.GetComponent<MeshRenderer>().bounds;
        wait_R = new BitArray2D(bound, child, agentRadius, 2);   
    }

    private void Update()
    {
        if (this.transform.hasChanged) {
            ziel.RefreshPos();
            wait_B.RefreshPos();
            wait_L.RefreshPos();
            wait_R.RefreshPos();
            this.transform.hasChanged = false;
        }
    }

    public Vector3Int GetNewPosition(AgentController ac)
    {
        Vector2Int cellCoord;
        int zone;
        CheckAuslastung();
        if (!ziel.IsFull())
        {
            cellCoord = ziel.FindBestPositionAndAdd(ac);
            zone = 0;
        }
        else if (!wait_B.IsFull())
        {
            cellCoord = wait_B.FindBestPositionAndAdd(ac);
            zone = 1;
        }
        else if (!wait_L.IsFull())
        {
            cellCoord = wait_L.FindBestPositionAndAdd(ac);
            zone = 2;
        }
        else if (!wait_R.IsFull())
        {
            cellCoord = wait_R.FindBestPositionAndAdd(ac);
            zone = 3;
        }else
        {
            //Needs to find another target
            return new Vector3Int(-1, -1, -1);
        }
        return new Vector3Int(cellCoord.x, cellCoord.y, zone);
    }

    public void CheckAuslastung()
    {
        komplettAusgelastet = ziel.IsFull() && wait_B.IsFull() && wait_L.IsFull() && wait_R.IsFull();
    }


    public void Reset()
    {
        Start();
    }
    public bool IstAusgelasted() { return komplettAusgelastet; }
}
