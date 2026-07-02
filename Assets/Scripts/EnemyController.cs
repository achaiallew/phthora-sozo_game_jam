using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    private NavMeshAgent nav;
    private List<Transform> patrolRoute;
    private int currentIndex = 0;

    void Awake()
    {
        nav = GetComponent<NavMeshAgent>();
    }

    public void SetPatrolRoute(List<Transform> route)
    {
        patrolRoute = route;

        if (patrolRoute != null && patrolRoute.Count > 0)
        {
            currentIndex = 0;
            nav.destination = patrolRoute[currentIndex].position;
        }
    }

    void Update()
    {
        if (patrolRoute == null || patrolRoute.Count == 0) return;

        if (!nav.pathPending && nav.remainingDistance < 0.5f)
        {
            currentIndex = (currentIndex + 1) % patrolRoute.Count;
            nav.destination = patrolRoute[currentIndex].position;
        }
    }
}