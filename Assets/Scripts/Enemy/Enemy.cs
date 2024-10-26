using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent), typeof(Damageable), typeof(Knockable))]
public class Enemy : MonoBehaviour
{
    [SerializeField] private float meleeDamage = 20f;
    [SerializeField] private float meleeKnockback = 10f;
    [SerializeField] private float meleeRange = 1.5f;
    [SerializeField] private float meleeCooldown = 2f;
    [SerializeField] private LayerMask meleeLayerMask;

    private NavMeshAgent agent;
    private Transform target;

    public Transform Target
    {
        set
        {
            target = value;
            StopAllCoroutines();
            StartCoroutine(PathUpdateRoutine());
            StartCoroutine(AttackRoutine());
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

    private IEnumerator AttackRoutine()
    {
        YieldInstruction cooldown = new WaitForSeconds(meleeCooldown);
        
        while (true)
        {
            float distance = Vector3.Distance(transform.position, target.position);
            if (distance > meleeRange)
            {
                yield return null;
                continue;
            }

            if (!Physics.CapsuleCast(transform.position - 0.4f * Vector3.up, transform.position + 0.4f * Vector3.up, 0.4f, (target.position - transform.position).normalized, out RaycastHit hit, meleeRange, meleeLayerMask))
            {
                yield return null;
                continue;
            }

            if (hit.transform != target)
            {
                // try to suicide
                yield return null;
                continue;
            }

            if (!hit.transform.TryGetComponent(out Damageable damageable))
            {
                yield return null;
                continue;
            }
            
            damageable.Damage(meleeDamage);

            if (hit.transform.TryGetComponent(out Knockable knockable))
            {
                knockable.ApplyKnockback(meleeKnockback * (hit.transform.position - transform.position).normalized);
            }
            
            yield return cooldown;
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
