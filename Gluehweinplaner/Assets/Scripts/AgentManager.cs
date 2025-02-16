using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class AgentManager : MonoBehaviour
{
    public int playerCount = 0;
    public int maxPlayerCount = 10;

    public bool simulating = true;

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

    public int GetNewCoords(AgentController ac, List<int> besuchteBudenNr)
    {
        
        for(int i = 0; i < alleBuden.Length; i++)
        {
            int rand = Random.Range(0, alleBuden.Length);
            
            if (!alleBuden[rand].IstAusgelasted() && !besuchteBudenNr.Contains(rand))
            {
                alleBuden[rand].GetNewPosition(ac);
                return rand;
            }
        }
        return -1;
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

    public void StartSimulation() { simulating = true; }

    public void StopSimulation() {  simulating = false; }

    public void ResetSimulation() {  simulating = false; foreach (Buden b in alleBuden) { b.Reset(); } }

   public void AddBude(Buden neueBude)
    {
        neueBude.Start();
        List<Buden> tempList = alleBuden.ToList();
        tempList.Add(neueBude);
        alleBuden = tempList.ToArray();
    }

    public int BudenCount() { return alleBuden.Length; }
    public int ExitCount() { return alleExits.Length; }

}