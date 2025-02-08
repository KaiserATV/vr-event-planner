using System;
using System.Collections;
using System.Collections.Generic;
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


  
    public BitArray2D( Bounds b, float aR, float x, float z) { 
        bounds = b;
        minX = x;
        minZ = z;
        //Debug.Log(minX + " "+ minZ);
        agentRadius = aR + 1.0f;
        full = false;   
        CalcWidthHeight();
        array = new BitArray(cellsX * cellsZ);
    }

    public void AddPlayer(Vector2Int v)
    {
        array[v.y * cellsX + v.x] = true;
    }

    public void RemovePlayer(Vector3Int v)
    {
        array[v.y * cellsX+ v.x] = false;
        full = false;
    }

    public Vector2Int FindBestPositionAndAdd()
    {
        for (int z = 0; z < cellsZ; z++)
        {
            for (int x = 0; x < cellsX; x++)
            {
                if (!array[z * cellsX + x])
                {
                    AddPlayer(new Vector2Int(x, z));
                    return new Vector2Int(x, z);
                }
            }
        }
        full = true;
        return new Vector2Int(-1, -1);
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

        return new Vector3(x, z, cells.z);
    }
}
