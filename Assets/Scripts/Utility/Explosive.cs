using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

public class Explosive : MonoBehaviour
{
    [SerializeField] private float damage;
    [SerializeField] private LayerMask damageMask;
    [SerializeField] private AnimationCurve damageFalloff;
    [SerializeField] private AnimationCurve knockbackFalloff;
    [SerializeField] private float splashRadius;
    [SerializeField] private float knockback;

    public void SplashDamage(Vector3 center)
    {
        foreach (Damageable target in ComponentUtility.GetComponentsInRadius<Damageable>(center, splashRadius, damageMask))
        {
            float distance = Vector3.Distance(center, target.transform.position);
            target.Damage(damage * damageFalloff.Evaluate(distance / splashRadius));
        }
    }

    public void KnockBack(Vector3 source)
    {
        foreach (Knockable target in ComponentUtility.GetComponentsInRadius<Knockable>(source, splashRadius))
        {
            Vector3 direction = target.transform.position - source;
            float distance = direction.magnitude;
            direction /= distance;
            target.ApplyKnockback(knockback * knockbackFalloff.Evaluate(distance / splashRadius) * direction);
        }
    }
}
