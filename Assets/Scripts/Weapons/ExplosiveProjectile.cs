using UnityEngine;

[RequireComponent(typeof(Explosive))]
public abstract class ExplosiveProjectile : Projectile
{
    private Explosive explosive;

    private void OnEnable()
    {
        explosive = GetComponent<Explosive>();
    }

    protected void Explode(Vector3 center)
    {
        explosive.Explode(center);
    }
}
