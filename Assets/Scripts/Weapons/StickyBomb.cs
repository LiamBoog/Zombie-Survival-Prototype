using System.Collections;
using UnityEngine;

public class StickyBomb : ExplosiveProjectile
{
    [SerializeField] private float countDownDuration = 2f;

    public override void Initialize()
    {
        base.Initialize();
        Impact += OnImpact;
    }

    public override void Deinitialize()
    {
        GetComponent<Rigidbody>().interpolation = RigidbodyInterpolation.Interpolate;
        Impact -= OnImpact;
    }

    private void OnImpact(ImpactInfo impactInfo)
    {
        StickToSurface(impactInfo);
        StartCoroutine(CountdownRoutine(impactInfo));
    }

    private void StickToSurface(ImpactInfo impactInfo)
    {
        Rigidbody rigidbody = GetComponent<Rigidbody>();
        rigidbody.velocity = Vector3.zero;
        rigidbody.isKinematic = true;
        rigidbody.interpolation = RigidbodyInterpolation.None;

        transform.position = impactInfo.point;
        transform.parent = impactInfo.target;
    }

    private IEnumerator CountdownRoutine(ImpactInfo impactInfo)
    {
        yield return new WaitForSeconds(countDownDuration);
        
        SplashDamage(impactInfo.point);
        KnockBack(impactInfo.point);
        Expire();
    }

    protected override float ComputeSplashDamage(Vector3 center, Vector3 target)
    {
        float distance = Vector3.Distance(target, center);
        float damageFallOff = -(1f - minDamageFallOff) * (distance - splashRadius) / splashRadius + minDamageFallOff;
        return damageFallOff * damage;
    }
}
