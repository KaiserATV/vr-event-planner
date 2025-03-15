using System.Collections.Generic;
using UnityEngine;

public class Exits : MonoBehaviour
{

    private Bounds b;

    private void Start()
    {
        b = GetComponent<MeshRenderer>().bounds;
    }


    public Vector3 GetClostestPoint()
    {
        return this.transform.position;
    }
}
