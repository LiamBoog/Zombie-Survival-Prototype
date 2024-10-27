using UnityEngine;

public class Rocket : ExplosiveProjectile
{
    [SerializeField] private AudioSource thrusterSound;
    
    public override void Initialize()
    {
        base.Initialize();
        thrusterSound.Play();
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
