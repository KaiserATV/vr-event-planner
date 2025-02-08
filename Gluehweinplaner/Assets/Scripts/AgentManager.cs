using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgentManager : MonoBehaviour
{
    public int playerCount = 0;
    public int maxPlayerCount = 10;
    Buden[] alleBuden;

    // Start is called before the first frame update
    void Start()
    {
        alleBuden = GameObject.Find("BudenContainer").GetComponentsInChildren<Buden>();
    }

    public Vector3Int GetNewCellsPos(out int budenNr)
    {
        for(int i = 0; i< alleBuden.Length; i++)
        {
            if (!alleBuden[i].IstAusgelasted())
            {
                budenNr = i;
                return alleBuden[i].GetNewPoisition();
            }
        }
        budenNr = -1;
        return new Vector3Int(-1,-1,-1);
    }

    public Vector3 GetNewWorldPos(Vector3Int v, int BudenNr)
    {
        return alleBuden[BudenNr].GetRealWorldCoords(v);
    }

    public void addPlayer(){ playerCount++;}
    public void removePlayer(){ playerCount--; }

    public bool CanAddPlayer() { return playerCount < maxPlayerCount; }

    public void AddBude(Buden neueBude) { alleBuden[alleBuden.Length] = neueBude;}
}
