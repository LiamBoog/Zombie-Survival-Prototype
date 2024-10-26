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
        base.Deinitialize();
        GetComponent<Rigidbody>().interpolation = RigidbodyInterpolation.Interpolate;
        Impact -= OnImpact;
    }

    private void OnImpact(ImpactInfo impactInfo)
    {
        StickToSurface(impactInfo);
        StartCoroutine(CountdownRoutine());
    }

    private void StickToSurface(ImpactInfo impactInfo)
    {
        Rigidbody rigidbody = GetComponent<Rigidbody>();
        rigidbody.isKinematic = true;
        rigidbody.interpolation = RigidbodyInterpolation.None;

        transform.position = impactInfo.point;
        transform.parent = impactInfo.target;
    }

    private IEnumerator CountdownRoutine()
    {
        yield return new WaitForSeconds(countDownDuration);
        
        Explode(transform.position);
        Expire();
    }
}
