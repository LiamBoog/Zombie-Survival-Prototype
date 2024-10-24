using UnityEngine;

public class Rocket : ExplosiveProjectile
{
    public override void Initialize()
    {
        base.Initialize();
        Impact += OnImpact;
    }

    public override void Deinitialize()
    {
        Impact -= OnImpact;
    }

    private void OnImpact(ImpactInfo impactInfo)
    {
        SplashDamage(impactInfo.point);
        KnockBack(impactInfo.point);

        Expire();
    }

    protected override float ComputeSplashDamage(Vector3 center, Vector3 target)
    {
        float distance = Vector3.Distance(target, center);
        float damageFallOff = (1f - minDamageFallOff) * Mathf.Pow(distance - splashRadius, 2f) / (splashRadius * splashRadius) + minDamageFallOff;
        return damageFallOff * damage;
    }
}
