using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZomblieSpawner : MonoBehaviour
{
    public GameObject ZombiePrefab;
    [Space(10)]
    public float MinSpawnRate;
    public float MaxSpawnRate;
    [Space(10)]
    public float Range;
    public float TriggerRange;
    [Space(10)]
    public int MinGroup;
    public int MaxGroup;
    [Space(10)]
    [Tooltip("Horde will spawn a fixed number and then stop forever")]
    public int TotalToSpawn;

    private float NextSpawn;
    private float TimeFromLastSpawn;

    void Start()
    {
        NextSpawn = Random.Range(MinSpawnRate, MaxSpawnRate);
    }

    void Update()
    {
        //Already spawned all I want too
        if(TotalToSpawn <= 0) return;

        //See if player in range 
        GameObject player_go = GameObject.FindGameObjectWithTag("Player");
        if(player_go==null) return;
        //If player outside of range then don't do anything
        if (Vector2.Distance(new Vector2(player_go.transform.position.x, player_go.transform.position.y),
        new Vector2(transform.position.x, transform.position.y)) > TriggerRange) return;

        TimeFromLastSpawn += Time.deltaTime;

        if (TimeFromLastSpawn > NextSpawn)
        {
            NextSpawn = Random.Range(MinSpawnRate, MaxSpawnRate);
            TimeFromLastSpawn = 0;

            int toSpawn = Random.Range(MinGroup, MaxGroup);
            if (toSpawn > TotalToSpawn) {
                toSpawn = TotalToSpawn;
            }
            TotalToSpawn -= toSpawn;

            for (int i = 0; i < toSpawn; i++)
            {
                Vector2 position = Random.insideUnitCircle * Range;
                Vector3 position3 = transform.position + new Vector3(position.x, position.y, 0);

                GameObject new_zombine = Instantiate(ZombiePrefab, position3, Quaternion.identity, transform);
                new_zombine.GetComponent<EnemyChase>().target = GameObject.FindWithTag("Player").transform;
            }

        }
    }
}
