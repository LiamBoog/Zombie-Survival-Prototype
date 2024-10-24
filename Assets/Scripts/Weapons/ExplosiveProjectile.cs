using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class ExplosiveProjectile : Projectile
{
    [SerializeField] private float damage = 10f;
    [SerializeField] private float splashRadius = 5f;

    private Action onFixedUpdate;
    
    protected void SplashDamage(Vector3 center)
    {
        IEnumerable<Damageable> targets = Physics.OverlapSphere(center, splashRadius)
            .Select(c => c.GetComponent<Damageable>())
            .Where(o => o != null);

        foreach (Damageable target in targets)
        {
            float distance = Vector3.Distance(target.transform.position, center);
            target.Damage(damage * distance / splashRadius);
        }
    }

    protected void KnockBack(Vector3 source)
    {
        IEnumerable<Knockable> targets = Physics.OverlapSphere(source, splashRadius)
            .Select(c => c.GetComponent<Knockable>())
            .Where(o => o != null);

        foreach (Knockable target in targets)
        {
            target.GetComponent<Rigidbody>().AddForce(10f * (target.transform.position - source).normalized, ForceMode.Impulse);
        }
    }

    private void FixedUpdate()
    {
        onFixedUpdate?.Invoke();
        onFixedUpdate = null;
    }
}
