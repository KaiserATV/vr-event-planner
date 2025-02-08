using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using UnityEngine;

public class Buden : MonoBehaviour
{
    public float agentRadius = 1.0f;

    public float waitTime = 50.0f;

    private bool komplettAusgelastet = false;

    //To-Do: Optimize the Datastructure
    private BitArray2D wait_B;
    private BitArray2D wait_L;
    private BitArray2D wait_R;
    private BitArray2D ziel;

    // Start is called before the first frame update
    void Start()
    {

        // !!!!IMPORTANT!!!! the number of the children specifies the position in the prefab, if changed, chang number here!!!!!!!
        // 3 - Wait_B, 4 - Wait_L, 5 - Wait_R, 6 - Ziel

        //ziel Array
        Transform child = this.transform.GetChild(6);
        Debug.Log(child);
        Debug.Log(child.transform.position + " " + this.transform.position);
        Bounds bound = child.GetComponent<MeshRenderer>().bounds;
        ziel = new BitArray2D(bound, agentRadius, child.transform.position.x - bound.extents.x, this.transform.position.z - bound.extents.z);

        //Wait_B Array
        child = this.transform.GetChild(3);
        bound = child.GetComponent<MeshRenderer>().bounds;
        wait_B = new BitArray2D(bound, agentRadius, child.transform.position.x-bound.extents.x, this.transform.position.z - bound.extents.z);

        //Wait_L Array
        child = this.transform.GetChild(4);
        bound = child.GetComponent<MeshRenderer>().bounds;
        wait_L = new BitArray2D(bound, agentRadius, child.transform.position.x - bound.extents.x, child.transform.position.z - bound.extents.z);

        //Wait_R Array
        child = this.transform.GetChild(5);
        bound = child.GetComponent<MeshRenderer>().bounds;
        wait_R = new BitArray2D(bound, agentRadius, child.transform.position.x - bound.extents.x, child.transform.position.z - bound.extents.z);

       
    }

    public Vector3Int GetNewPoisition()
    {
        Vector2Int cellCoord;
        int zone;
        if (!ziel.IsFull()) {
            cellCoord = ziel.FindBestPositionAndAdd();
            zone = 0;
        }
        else if (!wait_B.IsFull())
        {
            cellCoord = wait_B.FindBestPositionAndAdd();
            zone = 1;
        }
        else if (!wait_L.IsFull())
        {
            cellCoord = wait_L.FindBestPositionAndAdd();
            zone = 2;
        }else if (!wait_R.IsFull())
        {
            cellCoord = wait_R.FindBestPositionAndAdd();
            zone = 3;
        }
        else
        {
            komplettAusgelastet = true;
            //Needs to find another target
            return new Vector3Int(-1, -1, -1);
        }
        return new Vector3Int(cellCoord.x, cellCoord.y, zone);
    }

    public Vector3 GetRealWorldCoords(Vector3Int cells)
    {
        switch (cells.z)
        {
            case 0:
                return ziel.GetRealWorldCords(cells);
            case 1:
                return wait_B.GetRealWorldCords(cells);
            case 2:
                return wait_L.GetRealWorldCords(cells);
            case 3:
                return wait_R.GetRealWorldCords(cells);
            default:
                return new Vector3(-1, -1,cells.z);
        }
    }

    public void RemovePlayer(Vector3Int cells)
    {
        switch (cells.z)
        {
            case 0:
                ziel.RemovePlayer(cells);
                break;
            case 1:
                wait_B.RemovePlayer(cells);
                break;
            case 2:
                wait_L.RemovePlayer(cells);
                break;
            case 3:
                wait_R.RemovePlayer(cells);
                break;
        }
    }

    public bool IstAusgelasted() { return komplettAusgelastet; }
}
