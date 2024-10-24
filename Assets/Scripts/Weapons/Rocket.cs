using System;

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
}
