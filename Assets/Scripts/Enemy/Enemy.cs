using System.Collections;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent), typeof(Damageable))]
public class Enemy : MonoBehaviour
{
    private NavMeshAgent agent;
    private Transform target;

    public Transform Target
    {
        set
        {
            target = value;
            StopAllCoroutines();
            StartCoroutine(PathUpdateRoutine());
        }
    }
    
    private void OnEnable()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }

    private IEnumerator PathUpdateRoutine()
    {
        while (true)
        {
            agent.destination = target.position;
            yield return new WaitForSeconds(0.001f * Vector3.Distance(transform.position, target.position));
        }
    }
}
