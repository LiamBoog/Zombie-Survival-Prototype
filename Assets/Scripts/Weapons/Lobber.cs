using UnityEngine;

public class Lobber : Gun
{
    [SerializeField] private Rigidbody user;
    
    [SerializeField] private float stickyBombVelocity = 20f;
    
    protected override void InitializeProjectile(Projectile stickyBomb)
    {
        stickyBomb.Rigidbody.position = projectileSource.position;
        stickyBomb.Rigidbody.velocity = user.velocity + stickyBombVelocity * projectileSource.forward;
    }
}
