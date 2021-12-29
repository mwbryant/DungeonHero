using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomSpawn : MonoBehaviour
{    
    [Range(0.0f, 1.0f)]
    public float chestChance;
    public GameObject ChestPrefab;
    public GameObject EnemyPrefab;
    private bool spawned = false;
    public int MinSpawn = 1;
    public int MaxSpawn = 6;
    public float Range = 1;

    public void Spawn()
    {
        Debug.Log("Spawning!");
        float chance = Random.value;
            if(chance > chestChance){
            int toSpawn = Random.Range(MinSpawn, MaxSpawn);
            for(int i=0; i < toSpawn; i++){
                    //spawn Enemy
                    Vector2 position = Random.insideUnitCircle * Range;
                    Vector3 position3 = transform.position + new Vector3(position.x, position.y, 0);
                    Debug.Log("Spawning Enemy");
                    GameObject zombie = Instantiate(EnemyPrefab, position3, Quaternion.identity, transform);
                    zombie.GetComponent<EnemyChase>().target = GameObject.FindWithTag("Player").transform;

                }
            }else{
                Debug.Log("Spawning Chest");
                Instantiate(ChestPrefab, GetComponent<Transform>());
            }
            spawned = true;
    }

    public void OnTriggerEnter2D(Collider2D collider){
        if(collider.gameObject.tag == "Player" && !spawned){
            Spawn();
        }
    }

}
