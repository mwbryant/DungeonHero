using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;


//Scans the map for wall collisions and then runs A* pathfinding
public class NavGrid : MonoBehaviour
{
    [Tooltip("CAUTION: Small values will take a long time to run")]
    public float percision = 1;

    [HideInInspector]
    public bool ready = false;
    private bool[,] grid;
    private Vector2Int grid_dimensions;
    private Vector2 top_left;
    public LayerMask mask;

    readonly Vector2Int[] neighbor_offsets = new Vector2Int[4] { 
        new Vector2Int(-1, 0), 
        new Vector2Int(1,0),
        new Vector2Int(0,-1), 
        new Vector2Int(0,1)};

    public float HFunction(Vector2 pos, Vector2 end)
    {
        //XXX Giving the H function a little kick (*1.4) makes it greedier
        //This will not always give the shortest path but it greatly decreases run time
        //Think of it like exploring good routes more throughly instead of a full breathfirst search
        //And the goal isn't the mathematically shortest path, just a path
        return 1.4f * (Math.Abs(pos.x - end.x) + Math.Abs(pos.y - end.y));
    }

    public List<Vector2> PathFindingRequest(Vector2 start, Vector2 end)
    {
        float[,] f = new float[grid_dimensions.x, grid_dimensions.y];
        float[,] g = new float[grid_dimensions.x, grid_dimensions.y];
        for (int j = 0; j < grid_dimensions.y; j++)
            for (int i = 0; i < grid_dimensions.x; i++){
                g[i, j] = 100000;
                f[i, j] = 100000;
            }

        Vector2Int goal_index = PositionToIndex(end);

        Vector2Int start_index = PositionToIndex(start);
        g[start_index.x, start_index.y] = 0;
        f[start_index.x, start_index.y] = HFunction(start, end);

        List<Vector2Int> open_set = new List<Vector2Int>();
        open_set.Add(start_index);
        int searched = 0;

        while (open_set.Count != 0)
        {
            searched++;
            //This can be O(1) if using min heap instead of list but ehhh
            int min_index = -1;
            float min_f_score  = 10000000;
            //Find the most promising node and explore it
            for (int i = 0; i < open_set.Count; i++)
            {
                if (min_f_score > f[open_set[i].x, open_set[i].y])
                {
                    min_f_score = f[open_set[i].x, open_set[i].y];
                    min_index = i;
                }
            }
            if (open_set[min_index] == goal_index)
            {
                Debug.Log("MADE IT, searched " + searched);
                //TODO
                return new List<Vector2>();
            }
            //Check the neighbors of this tile
            for (int i = 0; i < neighbor_offsets.Length; i++){
                Vector2Int neighbor_index = new Vector2Int(open_set[min_index].x + neighbor_offsets[i].x, open_set[min_index].y + neighbor_offsets[i].y);
                if (neighbor_index.x < 0 || neighbor_index.y < 0 || neighbor_index.x >= grid_dimensions.x || neighbor_index.y >= grid_dimensions.y)
                {
                    Debug.Log("Failure, reached edge of map");
                    return new List<Vector2>();
                }
                if(!grid[neighbor_index.x, neighbor_index.y]) continue;

                //All neighbors are dist 1 away, no diagnols
                float tentative_g = g[open_set[min_index].x, open_set[min_index].y] + 1;
                if (tentative_g < g[neighbor_index.x, neighbor_index.y]) {
                    //This is a better way to reach neighbor, update
                    g[neighbor_index.x, neighbor_index.y] = tentative_g;
                    f[neighbor_index.x, neighbor_index.y] = tentative_g + HFunction(IndexToPosition(neighbor_index), end);
                    if (!open_set.Contains(neighbor_index))
                    {
                        open_set.Add(neighbor_index);
                    }
                }
            }
            //Removing from list might have O(n)....
            open_set.RemoveAt(min_index);
        }

        Debug.LogError("Pathfinding failure " + start + " to " + end);
        return new List<Vector2>();
    }

    Vector2Int PositionToIndex(Vector2 position)
    {
        int i = (int)((position.x - top_left.x - percision*.5f) / percision);
        int j = (int)((position.y - top_left.y + percision*.5f) / -percision);
        return new Vector2Int(i, j);
    }

    Vector3 IndexToPosition(Vector2Int index)
    {
        return new Vector3(top_left.x + index.x * percision + percision*.5f, top_left.y - index.y * percision - percision *.5f);
    }


    //Expensive function but called on start only...
    public void CreateGrid(Vector2 top_left_pos, Vector2 size)
    {
        top_left = top_left_pos;
        grid = new bool[(int)Math.Ceiling(size.x * 1 / percision), (int)Math.Ceiling(size.y * 1 / percision)];
        grid_dimensions = new Vector2Int(grid.GetLength(0), grid.GetLength(1));

        Debug.Log("Target " + mask.value);
        for (int j = 0; j < grid_dimensions.y; j++)
            for (int i = 0; i < grid_dimensions.x; i++)
            {
                //Test every point on the grid for a collider
                Vector2 location = IndexToPosition(new Vector2Int(i,j));
                RaycastHit2D hit = Physics2D.Raycast(location, Vector2.zero, Mathf.Infinity, mask, -100);
                //If the hit is on a wall then it is not traversable
                if(hit.collider != null) {
                        grid[i, j] = false;
                } else
                {
                    grid[i, j] = true;
                }
            }

        ready = true;
    }

    void Start()
    {
        CreateGrid(new Vector2(-90, 20), new Vector2(200, 200));
    }

    void OnDrawGizmosSelected()
    {
        if(grid == null) return;
        for (int j = 0; j < grid_dimensions.y; j++)
            for (int i = 0; i < grid_dimensions.x; i++)
            {
                if(grid[i,j])
                    Gizmos.color = Color.green;
                else
                    Gizmos.color = Color.red;
                Vector2 location = IndexToPosition(new Vector2Int(i,j));
                Vector3 size = new Vector3(percision, percision,1);
                Gizmos.DrawWireCube(location, size);
            }
    }
}
