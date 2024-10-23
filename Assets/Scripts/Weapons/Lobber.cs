using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Lobber : Gun
{
    [SerializeField] private Rigidbody user;
    
    [SerializeField] private float stickyBombVelocity = 20f;
    [SerializeField] private float stickyBombVerticalVelocity = 20f;
    
    protected override void InitializeProjectile(Projectile stickyBomb)
    {
        stickyBomb.transform.position = projectileSource.position;
        stickyBomb.Rigidbody.velocity = user.velocity + stickyBombVelocity * projectileSource.forward + stickyBombVerticalVelocity * Vector3.up;
    }
}
