// Alan Zucconi
// www.alanzucconi.com
using UnityEngine;

public class Heatmap : MonoBehaviour
{
    public bool showMax = false;


    public float[] properties;
    public int[] playCellCount;
    public int[] playMaxCount;

    private struct usageCat{
        public const int low = 3;
        public const int high = 6;
    }


    private struct alphaCat
    {
        public const float low = 0.5f;
        public const float medium = 0.75f;
        public const float high = 1f;
    }

    public Material material;


    private Bounds b;
    public float cellsizeX=10f;
    public float cellsizeZ=10f;
    public int cols;
    public int rows;


    void Start ()
    {
        b = GetComponent<MeshRenderer>().bounds;

        cols = Mathf.FloorToInt(b.size.x / cellsizeX);
        rows = Mathf.FloorToInt(b.size.z / cellsizeZ);

        //cellsizeX = b.size.x / cols;
        //cellsizeZ = b.size.z / rows;

        properties = new float[cols * rows];
        playCellCount = new int[cols * rows];
        playMaxCount = new int[cols * rows];

        material.SetInt("_Rows", rows);
        material.SetFloat("_XDistance", cellsizeX);
        material.SetFloat("_ZDistance", cellsizeZ);
        material.SetVector("_MaxVals", new Vector2(b.max.x, b.max.z));
    }

    void FixedUpdate()
    {
        material.SetFloatArray("_Properties", properties);
    }

    //Muss ausgeführt werden um auf maximal anzeige umzustellen
    public void showMaxAlpha()
    {
        for (int i = 0; i < properties.Length; i++)
        {
            properties[i] = determineAlpha(playMaxCount[i]);
        }
        showMax = true;
    }

    //Muss ausgeführt werden um wieder die aktuelle anzeige anzuzeigen
    public void showCurrentAlpha()
    {
        for (int i = 0; i < properties.Length; i++)
        {
            properties[i] = determineAlpha(playCellCount[i]);
        }
        showMax = false;
    }


    public Vector2Int Spawned(Vector2 worldPos)
    {
        Vector2Int cellCords = new Vector2Int();
        cellCords.x = Mathf.FloorToInt((b.max.x - worldPos.x) / cellsizeX);
        cellCords.y = Mathf.FloorToInt((b.max.z - worldPos.y) / cellsizeZ);
        int index = rows * cellCords.x + cellCords.y;
        playCellCount[index] += 1;
        int c = playCellCount[index];
        int cM = playMaxCount[index];
        if (c > cM) playMaxCount[index] = c;
        properties[index] = determineAlpha(showMax ? cM : c);
        return cellCords;
    }

    public Vector2Int Moved(Vector2Int from, Vector2 to)
    {
        int index1 = rows * from.x + from.y;
        Vector2Int newCells = new Vector2Int(Mathf.FloorToInt((b.max.x - to.x) / cellsizeX), Mathf.FloorToInt((b.max.z - to.y) / cellsizeZ));
        int index2 = rows * newCells.x + newCells.y;
        if (index1 != index2 && index1 >= 0)
        {
            playCellCount[index1] -= 1;
            playCellCount[index2] += 1;

            int c = playCellCount[index2];
            int cM = playMaxCount[index2];
                
            if ( c>cM ) playMaxCount[index2] = c;
            if (showMax)
            {
                properties[index1] = determineAlpha(playMaxCount[index1]);
                properties[index2] = determineAlpha(cM);
            }
            else
            {
                properties[index1] = determineAlpha(playCellCount[index1]);
                properties[index2] = determineAlpha(playCellCount[index2]);
            }
        }
        return newCells;
    }


    public float determineAlpha(int usage)
    {
        if(usage == 0)
        {
            return 0;
        }
        if(usage <= usageCat.low)
        {
            return alphaCat.low;
        }
        else
        {
            if (usage >= usageCat.high)
            {
                return alphaCat.high;
            }
            return alphaCat.medium;
        }
    }

}