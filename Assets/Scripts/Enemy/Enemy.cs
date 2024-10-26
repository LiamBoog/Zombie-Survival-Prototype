using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent), typeof(Damageable), typeof(Knockable))]
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
            yield return new WaitForSeconds(0.005f * Vector3.Distance(transform.position, target.position));
        }
    }

    public void KnockbackHandler(Vector3 impulse)
    {
        LayerMask enemyMask = LayerMask.GetMask(LayerMask.LayerToName(gameObject.layer));
        IEnumerable<Enemy> nearbyEnemies = Physics.OverlapSphere(transform.position, 2f, enemyMask)
            .Select(c => c.GetComponent<Enemy>())
            .Where(e => e != null);

        foreach (Enemy enemy in nearbyEnemies)
        {
            enemy.EnablePhysics(enemy == this ? impulse : null);
        }
    }

    private void EnablePhysics(Vector3? impulse)
    {
        StopAllCoroutines();
        Rigidbody rigidbody = GetComponent<Rigidbody>();
        StartCoroutine(TemporaryPhysics());

        IEnumerator TemporaryPhysics()
        {
            yield return null;

            agent.enabled = false;
            rigidbody.isKinematic = false;
            rigidbody.velocity = Vector3.zero;

            if (impulse.HasValue)
                rigidbody.AddForce(impulse.Value, ForceMode.Impulse);

            yield return new WaitForFixedUpdate();
            yield return new WaitUntil(() => rigidbody.velocity.magnitude < 0.1f);
            
            rigidbody.velocity = Vector3.zero;
            rigidbody.isKinematic = true;
            agent.Warp(transform.position);
            agent.enabled = true;
            StartCoroutine(PathUpdateRoutine());
        }
    }
}
