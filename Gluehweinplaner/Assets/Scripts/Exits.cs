using System.Collections.Generic;
using UnityEngine;

public class Exits : MonoBehaviour
{
    private List<Vector3> corners = new List<Vector3>();

    private Bounds b;

    private void Start()
    {
        b = GetComponent<MeshRenderer>().bounds;

        //Debug.Log(b.extents.x);

        //corners.Add(this.transform.localPosition + new Vector3(b.extents.x,0,-b.extents.z));
        //Debug.Log(corners[0])
        //corners.Add(this.transform.localPosition + new Vector3(b.extents.x, 0, b.extents.z));
        //corners.Add(this.transform.localPosition + new Vector3(-b.extents.x, 0, -b.extents.z));
        //corners.Add(this.transform.localPosition + new Vector3(-b.extents.x, 0, b.extents.z));

    }


    public Vector3 GetClostestPoint(Vector3 point)
    {
        //Vector3 closest= new Vector3(Mathf.Infinity, Mathf.Infinity);
        //closest = corners[0];
        //float currClostestDistance = Vector3.Distance(point, closest);

        //for(int i = 1; i < 4; i++)
        //{
        //    if(Vector3.Distance(point, corners[i]) < currClostestDistance)
        //    {
        //        closest = corners[i];
        //        currClostestDistance = Vector3.Distance(point, closest);
        //    }
        //}
        //Debug.Log(closest);
        return this.transform.position;
    }
}
