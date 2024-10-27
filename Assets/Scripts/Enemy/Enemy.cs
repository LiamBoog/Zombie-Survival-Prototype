using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent), typeof(Damageable), typeof(Knockable))]
[RequireComponent(typeof(Explosive), typeof(CapsuleCollider))]
public class Enemy : MonoBehaviour
{
    private const float VELOCITY_EPSILON = 0.1f;
    
    [SerializeField] private float meleeDamage = 20f;
    [SerializeField] private float meleeKnockback = 10f;
    [SerializeField] private float meleeRange = 1.5f;
    [SerializeField] private float meleeCooldown = 2f;
    [SerializeField] private LayerMask meleeLayerMask;

    [SerializeField] private float selfDestructRadius = 5f;
    [SerializeField] private float selfDestructProximityTime = 4f;
    [SerializeField, Range(0f, 1f)] private float selfDestructProbability = 0.3f;
    [SerializeField] private float selfDestructCooldown = 1f;
    [SerializeField] private float selfDestructCountdown = 2f;

    [SerializeField] private float pathfindingUpdateRate = 0.005f;
    [SerializeField] private float knockbackPropagationRadius = 2f;

    [SerializeField] private AudioSource fuseSound;

    private NavMeshAgent agent;
    private Transform target;
    private CapsuleCollider collider;

    public Transform Target
    {
        set
        {
            target = value;
            StopAllCoroutines();
            StartCoroutine(PathUpdateRoutine());
            StartCoroutine(MeleeRoutine());
            StartCoroutine(SelfDestructRoutine());
        }
    }
    
    private void OnEnable()
    {
        agent = GetComponent<NavMeshAgent>();
        collider = GetComponent<CapsuleCollider>();
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
            yield return new WaitForSeconds(pathfindingUpdateRate * Vector3.Distance(transform.position, target.position));
        }
    }

    private IEnumerator MeleeRoutine()
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

            Vector3 sphereOffset = (0.5f * collider.height - collider.radius) * Vector3.up;
            Vector3 point1 = transform.position - sphereOffset;
            Vector3 point2 = transform.position + sphereOffset;
            if (!Physics.CapsuleCast(point1, point2, collider.radius, (target.position - transform.position).normalized, out RaycastHit hit, meleeRange, meleeLayerMask))
            {
                yield return null;
                continue;
            }

            if (hit.transform != target)
            {
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

    private IEnumerator SelfDestructRoutine()
    {
        YieldInstruction cooldown = new WaitForSeconds(selfDestructCooldown);
        
        while (true)
        {
            float timer = selfDestructProximityTime;
            while (Vector3.Distance(transform.position, target.position) <= selfDestructRadius)
            {
                timer -= Time.deltaTime;
                if (timer > 0f)
                {
                    yield return null;
                    continue;
                }

                if (Random.Range(0f, 1f) < selfDestructProbability)
                {
                    yield return cooldown;
                    continue;
                }
                
                StopAllCoroutines();
                agent.enabled = false;
                StartCoroutine(CountdownRoutine());
                break;
            }

            yield return null;
        }

        IEnumerator CountdownRoutine()
        {
            fuseSound.Play();
            
            float timer = selfDestructCountdown;
            while (timer > 0f)
            {
                timer -= Time.deltaTime;
                yield return null;
            }

            GetComponent<Explosive>().Explode(transform.position);
            GetComponent<Damageable>().Damage(float.MaxValue);
        }
    }
    
    public void KnockbackHandler(Vector3 impulse)
    {
        LayerMask enemyMask = LayerMask.GetMask(LayerMask.LayerToName(gameObject.layer));
        IEnumerable<Enemy> nearbyEnemies = ComponentUtility.GetComponentsInRadius<Enemy>(transform.position, knockbackPropagationRadius, enemyMask);

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
            yield return new WaitUntil(() => rigidbody.velocity.magnitude < VELOCITY_EPSILON);
            
            rigidbody.velocity = Vector3.zero;
            rigidbody.isKinematic = true;
            agent.Warp(transform.position);
            agent.enabled = true;
            StartCoroutine(PathUpdateRoutine());
        }
    }
}
