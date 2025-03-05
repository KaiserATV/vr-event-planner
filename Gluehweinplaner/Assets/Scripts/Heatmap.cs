// Alan Zucconi
// www.alanzucconi.com
using UnityEngine;

public class Heatmap : MonoBehaviour
{

    public float[] properties;
    public int[] playCellCount;

    public struct usageCat{
        public const int low = 3;
        public const int high = 6;
    }

    public struct alphaCat
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

        material.SetInt("_Rows", rows);
        material.SetFloat("_XDistance", cellsizeX);
        material.SetFloat("_ZDistance", cellsizeZ);
        material.SetVector("_MinVals", new Vector2(b.min.x, b.min.z));
    }

    void FixedUpdate()
    {
        material.SetFloatArray("_Properties", properties);
    }


    public Vector2Int Spawned(Vector2 worldPos)
    {
        if (worldPos.x > b.min.x && worldPos.x < b.max.x)
        {
            if (worldPos.y > b.min.z && worldPos.y < b.max.z)
            {
                Vector2Int cellCords = new Vector2Int();
                cellCords.x = Mathf.FloorToInt((worldPos.x - b.min.x) / cellsizeX);
                cellCords.y = Mathf.FloorToInt((worldPos.y - b.min.z) / cellsizeZ);
                playCellCount[rows * cellCords.y + cellCords.x] += 1;
                properties[rows * cellCords.y + cellCords.x] = determineAlpha(playCellCount[rows *cellCords.y + cellCords.x]);
                return cellCords;
            }
        }
        return new Vector2Int(-1, -1);
    }

    public Vector2Int Moved(Vector2Int from, Vector2 to)
    {
        if (to.x > b.min.x && to.y < b.max.x)
        {
            playCellCount[rows * from.y + from.x] -= 1;
            properties[rows * from.y + from.x] = determineAlpha(playCellCount[rows * from.y + from.x]);

            Vector2Int newCells = new Vector2Int(Mathf.FloorToInt((to.x - b.min.x) / cellsizeX), Mathf.FloorToInt((to.y - b.min.z) / cellsizeZ));
            playCellCount[rows * newCells.y + newCells.x] += 1;
            properties[rows * newCells.y + newCells.x] = determineAlpha(playCellCount[rows * newCells.y + newCells.x]);
            return newCells;
        }
        return new Vector2Int(-1, -1);
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