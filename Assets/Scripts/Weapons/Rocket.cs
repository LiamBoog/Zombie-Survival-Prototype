public class Rocket : ExplosiveProjectile
{
    public override void Initialize()
    {
        base.Initialize();
        Impact += OnImpact;
    }

    public override void Deinitialize()
    {
        base.Deinitialize();;
        Impact -= OnImpact;
    }

    private void OnImpact(ImpactInfo impactInfo)
    {
        Explode(impactInfo.point);
        Expire();
    }
}
