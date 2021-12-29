using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

[Obsolete("Old Level Generator from Game Jam, use new Rouge gen instead")]
[RequireComponent(typeof(NavGrid))]
public class LevelGenerator : MonoBehaviour
{
    public Vector2 RoomSize = new Vector2(11, 6);
    public Vector2Int FloorSizeRange = new Vector2Int(2, 6);
    public Room DefaultRoom;
    //I would like to use a 2d array here
    //but alas unity hates me
    private Room[] Rooms;
    private GameObject[] Rooms_gos;
    public RoomConfig[] PlacedRooms;
    private int row_size;
    private Vector2Int center;

    public GameObject Enemy;


    void Generate()
    {
        //Decide on a floor size
        Vector2Int floor_size = new Vector2Int();
        floor_size.x = Random.Range(FloorSizeRange.x, FloorSizeRange.y);
        floor_size.y = Random.Range(FloorSizeRange.x, FloorSizeRange.y);

        //Put the end randomly at the corner of the floor
        Vector2Int end_position = floor_size - new Vector2Int(1,1);
        if(Random.Range(0.0f, 1.0f) < .5f) end_position.y = -end_position.y;
        if(Random.Range(0.0f, 1.0f) < .5f) end_position.x = -end_position.x;

        //Track placed rooms
        //Set size of array to be how far the end was placed
        center = new Vector2Int(floor_size.x , floor_size.y);
        row_size = (floor_size.x * 2 + 1);
        PlacedRooms = new RoomConfig[row_size * (floor_size.y * 2 + 1)];

        ShuffleRooms();
        //Get a start room
        RoomConfig start_config = new RoomConfig(end_position.x > 0, end_position.x < 0, end_position.y > 0, end_position.y < 0);
        for (int i = 0; i < Rooms.Length; i++)
        {
            if (Rooms[i].Start && start_config.CheckRoom(Rooms[i]))
            {
                Room start = Rooms[i];
                Transform start_prefab = Rooms_gos[i].transform;
                Instantiate(start_prefab, new Vector3(0, 0, 0), Quaternion.identity, transform);
                PlacedRooms[Index(center.x,  center.y)] = 
                    new RoomConfig(start.RightExit, start.LeftExit, start.TopExit, start.BottomExit);
                break;
            }
        }

        ShuffleRooms();
        //Get a boss room
        RoomConfig end_config = new RoomConfig(end_position.x < 0, end_position.x > 0, end_position.y < 0, end_position.y > 0);
        for (int i = 0; i < Rooms.Length; i++)
        {
            if (Rooms[i].Boss && end_config.CheckRoom(Rooms[i]))
            {
                Room boss = Rooms[i];
                Transform boss_prefab = Rooms[i].transform;
                Instantiate(boss_prefab, new Vector3(end_position.x*RoomSize.x, end_position.y*RoomSize.y, 0), Quaternion.identity, transform);
                PlacedRooms[Index(end_position.x + center.x, end_position.y + center.y)] = 
                    new RoomConfig(boss.RightExit, boss.LeftExit, boss.TopExit, boss.BottomExit);
                break;
            }
        }

        ConnectRoom(Vector2Int.zero, end_position);

        //Fill out open spots
        for (int j = 0; j < floor_size.y * 2 +1; j++)
            for (int i = 0; i < floor_size.x * 2 +1; i++) {
                if (PlacedRooms[Index(i, j)] == null)
                {
                    RoomConfig roomConf = new RoomConfig(false, false,false,false);
                    //Check if any of my neighbors left an open door and plug it
                    if (i != 0 && PlacedRooms[Index(i - 1, j)] != null && PlacedRooms[Index(i - 1, j)].needs_right)
                        roomConf.needs_left = true;
                    if (j != 0 && PlacedRooms[Index(i , j-1)] != null && PlacedRooms[Index(i , j-1)].needs_up)
                        roomConf.needs_down = true;
                    if (i != floor_size.x*2  && PlacedRooms[Index(i + 1, j)] != null && PlacedRooms[Index(i + 1, j)].needs_left)
                        roomConf.needs_right = true;
                    if (j != floor_size.y * 2 && PlacedRooms[Index(i , j+1)] != null && PlacedRooms[Index(i , j+1)].needs_down)
                        roomConf.needs_up = true;

                    //If theres nothing to add then dont spawn a room
                    if (roomConf.needs_left || roomConf.needs_right || roomConf.needs_down || roomConf.needs_up)
                    {
                        SpawnRoom(roomConf, i - center.x, j - center.y);
                        PlacedRooms[Index(i, j)] = roomConf;
                    }
                }
            }
        GetComponent<NavGrid>().CreateGrid(new Vector2(transform.position.x - (RoomSize.x * floor_size.x), transform.position.y + (RoomSize.y * floor_size.y)), new Vector2(RoomSize.x * (floor_size.x*2+1), RoomSize.y * (floor_size.y*2+1)));
    }

    void ConnectRoom(Vector2Int start, Vector2Int end)
    {
        int sad_failure = 0;
        while ((start.x != end.x || start.y != start.y) && sad_failure < 100)
        {
            RoomConfig roomConf = new RoomConfig((end.x > start.x), (end.x < start.y), (end.y > start.y), (end.y < start.y));
            sad_failure++;
            //If a room is never placed (ie randomly wanted to move x but was already where i needed to be)
            //Then dont do anything and re-loop
            bool placed_room = false;
            if (Random.Range(0.0f, 1.0f) < .5f) {
                //Find a room that fits
                if (end.x > start.x)
                {
                    //Connecting on left
                    roomConf.needs_right = (end.x > (start.x + 1));
                    roomConf.needs_left = true;
                    start.x++;
                    placed_room = true;
                } else
                if (end.x < start.x)
                {
                    //Connecting on right
                    roomConf.needs_right = true;
                    roomConf.needs_left = (end.x < (start.x-1));
                    start.x--;
                    placed_room = true;
                }
            }else{
                if (end.y > start.y)
                {
                    //Connecting on bottom
                    roomConf.needs_down = true;
                    roomConf.needs_up = (end.y > (start.y+1));
                    start.y++;
                    placed_room = true;
                } else
                if (end.y < start.y)
                {
                    //Connecting on top
                    roomConf.needs_up = true;
                    roomConf.needs_down = (end.y < (start.y-1));
                    start.y--;
                    placed_room = true;
                }
            }
            if(start.x == end.x && start.y==end.y) continue;
            // spawn room
            if (placed_room)
            {
                SpawnRoom(roomConf, start.x, start.y);
                PlacedRooms[Index(start.x + center.x, start.y + center.y)] = roomConf;
            }
        }
        if(sad_failure >99) Debug.Log("I failed to make a map :(");
    }

    void SpawnRoom(RoomConfig roomConfig, int target_x, int target_y)
    {
        ShuffleRooms();
        bool found_room = false;
        for (int i = 0; i < Rooms.Length; i++)
        {
            if(roomConfig.needs_right !=Rooms[i].RightExit) continue;
            if(roomConfig.needs_left !=Rooms[i].LeftExit) continue;
            if(roomConfig.needs_up !=Rooms[i].TopExit) continue;
            if(roomConfig.needs_down !=Rooms[i].BottomExit) continue;
            if(Rooms[i].Start || Rooms[i].Boss) continue;
            //this room is good
            Transform new_room = Instantiate(Rooms_gos[i].transform, new Vector3(target_x*RoomSize.x, target_y*RoomSize.y, 0), Quaternion.identity, transform);
            SetTheme(new_room.gameObject.transform.gameObject, Enemy);
            found_room = true;
            break;
        }
        if(!found_room) Debug.LogError("Help i couldnt find a room " + roomConfig.needs_up + " " + roomConfig.needs_down + " "+  roomConfig.needs_left + " "+ roomConfig.needs_right);
    }

    void SetTheme(GameObject toSet, GameObject enemy, int depth = 0)
    {
        if(depth > 5) {
            return;
        }

        if (toSet.GetComponent<RoomSpawn>() != null)
        {
            toSet.GetComponent<RoomSpawn>().EnemyPrefab = enemy;
        }
        //recurse
        foreach(Transform child in toSet.transform)
        {
            if (child.GetInstanceID() == toSet.transform.GetInstanceID())
            {
                Debug.Log("I am my own child");
                continue;
            }
            SetTheme(child.gameObject, enemy, depth+1);
        }
    }

    void Start() {
        //Loads everything in the resources folder with the room componenet
        //Saves Isaac from having to set up rooms on the prefab
        Rooms_gos = Resources.LoadAll<GameObject>("Rooms");
        Rooms = new Room[Rooms_gos.Length];

        for (int i = 0; i < Rooms_gos.Length; i++)
        {
            if (Rooms_gos[i].GetComponent<Room>() == null)
            {
                Debug.LogError("Please remember to attached the room script to all prefabs in the rooms folder!!");
                Debug.LogError("Bad Object " + Rooms_gos[i].name);
                Debug.LogError("Aborting level generation");
                return;
            }
            Rooms[i] = Rooms_gos[i].GetComponent<Room>();
        }

        Generate();
    }

    int Index(int x, int y)
    {
        return x + y * row_size;
    }

    void ShuffleRooms()
    {
        for (int i = 0; i < 10; i++)
        {
            for (int n = 0; n < Rooms.Length; n++)
            {
                int target = Random.Range(0, Rooms.Length);

                Room to_swap = Rooms[target];
                GameObject to_swap_go = Rooms_gos[target];

                Rooms[target] = Rooms[n];
                Rooms_gos[target] = Rooms_gos[n];

                Rooms[n] = to_swap;
                Rooms_gos[n] = to_swap_go;
            }
        }
    }
}

[Serializable]
public class RoomConfig {
    public bool needs_right;
    public bool needs_left;
    public bool needs_up;
    public bool needs_down;

    public RoomConfig(bool _needs_right, bool _needs_left, bool _needs_up, bool _needs_down) {
        needs_right = _needs_right;
        needs_left = _needs_left;
        needs_up = _needs_up;
        needs_down = _needs_down;
    }

    public bool CheckRoom(Room toCheck)
    {
        if(needs_right && !toCheck.RightExit) return false;
        if(needs_left && !toCheck.LeftExit) return false;
        if(needs_up && !toCheck.TopExit) return false;
        if(needs_down && !toCheck.BottomExit) return false;
        return true;
    }
}