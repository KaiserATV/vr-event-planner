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
    private List<AgentController> registeredPlayers=new List<AgentController>();
    private Transform childT;
    private int positionToBude; //0 -directly infront of Bode, 1 - to the left of the Bude, 2- to the right of the Bude
    private Vector3 localSize;
    private float schiebX;
    private float schiebZ;

    public BitArray2D( Bounds b, Transform child, float aR, int p) { 
        bounds = b;
        childT = child;
        agentRadius = aR;
        full = false;   
        CalcWidthHeight();
        positionToBude = p;
        array = new BitArray(cellsX * cellsZ);
        localSize = childT.InverseTransformPoint(bounds.max) - childT.InverseTransformPoint(bounds.min);
        schiebX = Mathf.Abs(localSize.x / cellsX);
        schiebZ = Mathf.Abs(localSize.z / cellsZ);
    }

    public void AddPlayer(Vector2Int v, AgentController ac)
    {
        registeredPlayers.Add(ac);
        ac.SetCells(v);
        ac.SetBude(this);
        ac.SetGoal(GetRealWorldCords(v));
        array[v.y * cellsX + v.x] = true;
        if(registeredPlayers.Count == cellsX * cellsZ) { full = true; }
    }

    public void RemovePlayer(Vector2Int v, AgentController ac)
    {
        if(registeredPlayers.Contains(ac)){ registeredPlayers.Remove(ac); };
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
        for (int x = 0; x < cellsX; x++)
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
        for (int x = 0; x < cellsX; x++)
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

    public Vector2Int AddToRight(AgentController ac)
    {
        for (int x = 0; x < cellsX; x++)
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



    public void RefreshPos()
    {
        foreach (AgentController ac in registeredPlayers)
        {
            ac.InvalidatePosition(GetRealWorldCords(ac.GetCells()));
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

    public Vector2 GetRealWorldCords(Vector2Int cells)
    {
        float lx = (cellsX / 2 * schiebX) - (cells.x * schiebX) - schiebX / 2;
        float lz = (cellsZ / 2 * schiebZ) - (cells.y * schiebZ) - schiebZ / 2;
        Vector3 tV = childT.TransformPoint(new Vector3(lx, 0, lz));
        return new Vector3(tV.x, tV.z);
    }
}
