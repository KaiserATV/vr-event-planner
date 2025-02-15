using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

public class AgentManager : MonoBehaviour
{
    public int playerCount = 0;
    public int maxPlayerCount = 10;

    public string budenContainerName = "BudenContainer";
    public string exitContainerName = "ExitContainer";

    Buden[] alleBuden;
    Exits[] alleExits;

    // Start is called before the first frame update
    void Start()
    {
        alleBuden = GameObject.Find(budenContainerName).GetComponentsInChildren<Buden>();
        alleExits = GameObject.Find(exitContainerName).GetComponentsInChildren<Exits>();
    }

    public Vector3Int GetNewCellsPos(out int budenNr, AgentController ac)
    {
            for (int i = 0; i < alleBuden.Length; i++)
            {
                if (!alleBuden[i].IstAusgelasted())
                {
                    budenNr = i;
                    return alleBuden[i].GetNewPoisition(ac);
                }
            }
            budenNr = -1;
            return new Vector3Int(-1, -1, -1);
    }

    public void DeRegisterPlayer(AgentController ac, Vector3Int cells,int goalNr)
    {
        alleBuden[goalNr].RemovePlayer(ac, cells);
    }

    public Vector3 GetNewWorldPos(Vector3Int v, int nr)
    {
            return alleBuden[nr].GetRealWorldCoords(v);
    }

    public Vector3 GetClostestExit(Vector3 position)
    {
        Vector3 exitCoords = Vector3.zero;
        if(alleExits.Length > 0)
        {
            exitCoords = alleExits[0].GetClostestPoint(position);
            float currClostestDistance = Vector3.Distance(position,exitCoords);
            for (int i = 1; i < alleExits.Length; i++)
            {
                if (Vector3.Distance(position, alleExits[i].GetClostestPoint(position)) < currClostestDistance)
                {
                    exitCoords = alleExits[i].GetClostestPoint(position);
                    currClostestDistance = Vector3.Distance(position, exitCoords);
                }
            }
        }
        return new Vector3(exitCoords.x,exitCoords.z,0);
    }

    public float GetWaitTime(int budenNr)
    {
        return alleBuden[budenNr].waitTime;
    }

    public void addPlayer(){ playerCount++;}
    public void removePlayer(){ playerCount--; }

    public bool CanAddPlayer() {return (playerCount < maxPlayerCount); }

    public void AddBude(Buden neueBude) { alleBuden[alleBuden.Length] = neueBude;}
}
