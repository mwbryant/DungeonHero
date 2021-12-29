using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Dummy testers to use the NavGrid pathfinding
public class PathFindingTester : MonoBehaviour
{
    public NavGrid grid;
    public Vector2 start;
    public Vector2 end;

    void Update()
    {
        if(!grid.ready) return;
        grid.PathFindingRequest(start, end);
    }
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Vector3 size = new Vector3(1, 1,1);
        Gizmos.DrawWireCube(start, size);
        Gizmos.DrawWireCube(end, size);
    }
}
