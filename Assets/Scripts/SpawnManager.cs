using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SpawnManager : MonoBehaviour
{
    [SerializeField] private List<Transform> enemySpawns = new List<Transform>();
    [SerializeField] private List<Transform> patrolRoutes = new List<Transform>(); // drag Route_A, Route_B, etc. here
    [SerializeField] private GameObject enemyType1;

    void Awake()
    {
        for (int i = 0; i < enemySpawns.Count; i++)
        {
            Transform spawn = enemySpawns[i];

            if (NavMesh.SamplePosition(spawn.position, out NavMeshHit hit, 5f, NavMesh.AllAreas))
            {
                GameObject enemy = Instantiate(enemyType1, spawn.position, spawn.rotation);
                EnemyController controller = enemy.GetComponent<EnemyController>();

                // Build the route list from this spawn's matching Route_X children
                if (i < patrolRoutes.Count)
                {
                    List<Transform> route = new List<Transform>();
                    foreach (Transform point in patrolRoutes[i])
                    {
                        route.Add(point);
                    }
                    controller.SetPatrolRoute(route);
                }
                else
                {
                    Debug.LogWarning($"No patrol route assigned for spawn index {i} ({spawn.name})");
                }
            }
            else
            {
                Debug.LogWarning($"Spawn point {spawn.name} is too far from the NavMesh — check its position.");
            }
        }
    }
}