using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class ExplosiveProjectile : Projectile
{
    [SerializeField] protected float damage;
    [SerializeField, Range(0f, 1f)] protected float minDamageFallOff;
    [SerializeField] protected float splashRadius;
    [SerializeField] protected float knockback;

    protected abstract float ComputeSplashDamage(Vector3 center, Vector3 target);

    protected virtual Vector3 ComputeKnockback(Vector3 source, Vector3 target)
    {
        Vector3 direction = (target - source).normalized;
        return knockback * direction;
    }
    
    protected void SplashDamage(Vector3 center)
    {
        foreach (Damageable target in GetTargets<Damageable>(center))
        {
            target.Damage(ComputeSplashDamage(center, target.transform.position));
        }
    }

    protected void KnockBack(Vector3 source)
    {
        foreach (Knockable target in GetTargets<Knockable>(source))
        {
            target.ApplyKnockback(ComputeKnockback(source, target.transform.position));
        }
    }

    private IEnumerable<T> GetTargets<T>(Vector3 center) where T : Component
    {
        return Physics.OverlapSphere(center, splashRadius)
            .Select(c => c.GetComponent<T>())
            .Where(o => o != null);
    }
}
