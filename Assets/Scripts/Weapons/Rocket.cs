using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Rocket : Projectile
{
    [SerializeField] private float damage = 10f;
    [SerializeField] private float splashRadius = 5f;

    private Action knockback;
    
    protected override void Initialize()
    {
        Impact += OnImpact;
    }

    protected override void Deinitialize()
    {
        Impact -= OnImpact;
    }

    private void OnImpact(Vector3 contact)
    {
        SplashDamage(contact);
        knockback = () =>
        {
            KnockBack(contact);
            knockback = null;
        };
    }

    private void FixedUpdate()
    {
        knockback?.Invoke();
    }

    private void SplashDamage(Vector3 center)
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

    private void KnockBack(Vector3 source)
    {
        IEnumerable<Knockable> targets = Physics.OverlapSphere(source, splashRadius)
            .Select(c => c.GetComponent<Knockable>())
            .Where(o => o != null);

        foreach (Knockable target in targets)
        {
            Debug.Log(target.name);
            target.GetComponent<Rigidbody>().AddForce(1000f * (target.transform.position - source).normalized);
        }
    }
}
