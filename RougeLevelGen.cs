using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public struct RoomLayout {
    List<Vector2> exits;
}

//TODO very unfinished and hacky currently
public class RougeLevelGen : MonoBehaviour
{
    private GameObject[] roomGOs;

    void Start() {
        roomGOs = Resources.LoadAll<GameObject>("RougeRooms");
        Debug.Log("Found " + roomGOs.Length + " Rooms");

        Generate();
    }

    RoomLayout FindExits(GameObject room) {
        TilemapCollider2D[] maps = room.GetComponentsInChildren<TilemapCollider2D>();
        Debug.Log("Found " + maps.Length + " Tilemap colliders in " + room.name);
        bool found_walls = false;
        for (int i = 0; i < maps.Length; i++) {
            //TODO remove string here
            if (maps[i].gameObject.layer == LayerMask.NameToLayer("Wall")) {
                found_walls = true;
                Tilemap map = maps[i].gameObject.GetComponent<Tilemap>();
                TileBase[] tiles = map.GetTilesBlock(map.cellBounds);
                for (int x = 0; x < map.size.x; x++) {
                        if (tiles[x + map.size.x * (map.size.y-1)] == null) {
                            Debug.Log("top tile gap at " + x);
                        }
                }
            }
        }
        if(!found_walls) Debug.LogWarning("Failed to find any tilemap colliders on the wall layer for " + room.name);
        return new RoomLayout();
    }

    void Generate() {
        List<RoomLayout> layouts = new List<RoomLayout>(roomGOs.Length);

        for (int i = 0; i < roomGOs.Length; i++) {
            layouts.Add(FindExits(roomGOs[i]));
        }
    }
}
