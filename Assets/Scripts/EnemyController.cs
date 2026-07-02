using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

//TODO: Enemy Movement
public class EnemyController : MonoBehaviour
{
    private NavMeshAgent nav;
    [SerializeField] private List<Transform> enemyPatrols = new List<Transform>();
    private GameObject[] enemies;
    void Start()
    {
        nav = GetComponent<NavMeshAgent>();
        enemies = GameObject.FindGameObjectsWithTag("Enemy");

        Transform patrolParent = GameObject.Find("Enemy Patrol Locations").transform;
        foreach (Transform child in patrolParent)
        {
            enemyPatrols.Add(child);
        }

    }

    // // Update is called once per frame
    void Update()
    {
    
    }
 }
