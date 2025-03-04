// Alan Zucconi
// www.alanzucconi.com
using UnityEngine;

public class Heatmap : MonoBehaviour
{

    public Vector4[] positions;
    public Vector4[] properties;


    public Material material;


    private Bounds b;
    public float cellsizeX;
    public float cellsizeZ;
    public int cols = 31;
    public int rows = 31;
    public const float personInfluence = 0.5f;

    private int prevPlayerCount;


    void Start ()
    {
        b = GetComponent<MeshRenderer>().bounds;
        
        cellsizeX = b.size.x / cols;
        cellsizeZ = b.size.z / rows;

        positions = new Vector4[cols * rows];
        properties = new Vector4[cols * rows];


        int z = 0;
        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
            {
                positions[z] = new Vector4((j * cellsizeX) + b.min.x +cellsizeX/2, 0, (i * cellsizeZ) + b.min.z+cellsizeZ / 2, 0);
                z++;
            }
        }

        for (int i = 0; i < positions.Length; i++)
        {
            properties[i] = new Vector4(Mathf.Min(cellsizeZ,cellsizeX)/2,0,0, 0);
        }
    
        
        material.SetInt("_Points_Length", cols * rows);
        material.SetVectorArray("_Points", positions);
        material.SetVectorArray("_Properties", properties);

    }

    void FixedUpdate()
    {
        material.SetVectorArray("_Properties", properties);
    }

    public Vector2Int Spawned(Vector2 worldPos, int count)
    {
        prevPlayerCount = count;
        if (worldPos.x > b.min.x && worldPos.x < b.max.x)
        {
            if (worldPos.y > b.min.z && worldPos.y < b.max.z)
            {
                Vector2Int cellCords = new Vector2Int();
                cellCords.x = Mathf.FloorToInt((worldPos.x - b.min.x) / cellsizeX);
                cellCords.y = Mathf.FloorToInt((worldPos.y - b.min.z) / cellsizeZ);
                properties[rows * cellCords.y + cellCords.x].y += personInfluence;
                /// prevPlayerCount;
                return cellCords;
            }
        }
        return new Vector2Int(-1, -1);
    }

    public Vector2Int Moved(Vector2Int from, Vector2 to, int count)
    {
        int tmp;
        if (!(from.x < 0 || from.y < 0)) {
            properties[rows * from.y + from.x].y -= personInfluence; 
        }
        if (to.x > b.min.x && to.y < b.max.x)
        {
            Vector2Int newCells = new Vector2Int(Mathf.FloorToInt((to.x - b.min.x) / cellsizeX), Mathf.FloorToInt((to.y - b.min.z) / cellsizeZ));
            tmp = Mathf.FloorToInt(properties[rows * newCells.y + newCells.x].y * prevPlayerCount);
            properties[rows * newCells.y + newCells.x].y += personInfluence;
            //(tmp + 1) / count;
            //prevPlayerCount = count;
            return newCells;
        }
        prevPlayerCount = count;
        return new Vector2Int(-1, -1);
    }



}