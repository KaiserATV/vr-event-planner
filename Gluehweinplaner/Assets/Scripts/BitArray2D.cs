using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

public class BitArray2D
{

    //z=0x=0 z=0x=1
    //z=1x=0 z=1x=1
    private BitArray array;

    private Bounds bounds;
    private float agentRadius;
    private bool full;
    private int cellsX;
    private int cellsZ;
    private float minX;
    private float minZ;
    private List<AgentController> registeredPlayers=new List<AgentController>();
    private Transform childT;
    private int positionToBude; //0 -directly infront of Bode, 1 - to the left of the Bude, 2- to the right of the Bude
    private float rotation;
    private Vector3 pivot;


    public BitArray2D( Bounds b, Transform child, float aR, int p, float r, Vector3 budenPivot) { 
        bounds = b;
        childT = child;
        minX = child.transform.position.x - bounds.extents.x;
        minZ = child.transform.position.z - bounds.extents.z;
        agentRadius = aR;
        full = false;   
        CalcWidthHeight();
        rotation = r;
        positionToBude = p;
        pivot = budenPivot;
        array = new BitArray(cellsX * cellsZ);
    }

    public void AddPlayer(Vector2Int v, AgentController ac)
    {
        registeredPlayers.Add(ac);
        array[v.y * cellsX + v.x] = true;
        if(registeredPlayers.Count == cellsX * cellsZ) { full = true; }
    }

    public void RemovePlayer(Vector3Int v, AgentController ac)
    {
        registeredPlayers.Remove(ac);
        array[v.y * cellsX+ v.x] = false;
        full = false;
    }

    public Vector2Int FindBestPositionAndAdd(AgentController ac)
    {
        if(!full)
        {
            switch (positionToBude)
            {
                case 0:
                    return AddInFront(ac); 
                case 1:
                    return AddToLeft(ac); 
                case 2: 
                    return AddToRight(ac);
            }
        }
        return new Vector2Int(-1, -1);
    }

    public Vector2Int AddInFront(AgentController ac)
    {
        for (int x = cellsX-1; x >= 0; x--)
        {
            for (int z = 0; z < cellsZ; z++)
            {
                if (!array[z * cellsX + x])
                {
                    AddPlayer(new Vector2Int(x, z), ac);
                    return new Vector2Int(x, z);
                }
            }
        }
        return new Vector2Int(-1, -1);
    }

    public Vector2Int AddToLeft(AgentController ac)
    {
        for (int x = cellsX - 1; x >= 0; x--)
        {
            for (int z = 0; z < cellsZ; z++)
            {
                if (!array[z * cellsX + x])
                {
                    AddPlayer(new Vector2Int(x, z), ac);
                    return new Vector2Int(x, z);
                }
            }
        }
        return new Vector2Int(-1, -1);
    }

    public Vector2Int AddToRight(AgentController ac)
    {
        for (int x = cellsX - 1; x >= 0; x--)
        {
            for (int z = cellsZ-1; z >= 0; z--)
            {
                if (!array[z * cellsX + x])
                {
                    AddPlayer(new Vector2Int(x, z), ac);
                    return new Vector2Int(x, z);
                }
            }
        }
        return new Vector2Int(-1, -1);
    }



    public void RefreshPos(float r, Vector3 p)
    {
        rotation = r;
        foreach (AgentController ac in registeredPlayers)
        {
            ac.InvalidatePosition(GetRealWorldCords(ac.cells));
        }
    }


    public bool IsFull()
    {
        return full;
    }

    private void CalcWidthHeight()
    {
        cellsX = Mathf.FloorToInt(bounds.size.x / agentRadius);
        cellsZ = Mathf.FloorToInt(bounds.size.z / agentRadius);
    }

    public Vector3 GetRealWorldCords(Vector3Int cells)
    {
        float x = minX + (cells.x * agentRadius) + (agentRadius / 2);
        float z = minZ + (cells.y * agentRadius) + (agentRadius / 2);


        //ToDo Add movability

        Vector3 rotatedVector = RotatePointAroundPivot(new Vector3(x, 0, z));
        return new Vector3(rotatedVector.x,rotatedVector.z,cells.z);
    }
  
    public Vector3 RotatePointAroundPivot(Vector3 point){
        Vector3 dir = point - pivot; // get point direction relative to pivot
        dir = Quaternion.Euler(0, rotation, 0) * dir; // rotate it
        point = dir + pivot; // calculate rotated point
        return point; // return it
    }



}
