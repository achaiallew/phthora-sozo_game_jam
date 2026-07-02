using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{

    [SerializeField] private List<Transform> enemySpawns = new List<Transform>();
    [SerializeField] private GameObject enemyType1;

    void Awake()
    {
        // Instantiate an enemy at each spawn transform's position and rotation
        enemySpawns.ForEach(spawn => Instantiate(enemyType1, spawn.position, spawn.rotation));
        // for (int i = 0; i < enemySpawns.Count; i++)
        // {
        //     Instantiate(enemyType1, enemySpawns[i].position, enemySpawns[i].rotation);
        // }
    }

    void Update()
    {
        
    }
}
