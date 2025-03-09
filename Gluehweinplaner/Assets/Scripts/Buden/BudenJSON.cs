using System;

[Serializable]
public class BudenJSON
{
    public float xPos;
    public float zPos;
    public float yRot;
    public int typeIndex;

    public BudenJSON(float x, float z,float r, int i)
    {
        xPos = x;
        zPos = z;
        yRot = r;
        typeIndex = i;
    }
}

[Serializable]
public class AlleBudenJSON
{
    public BudenJSON[] budenArray;
    public AlleBudenJSON(int s)
    {
        budenArray = new BudenJSON[s];
    }
}

