using UnityEngine;

public class Bazooka : Gun
{
    [SerializeField] private new Camera camera;

    [SerializeField] private float rocketVelocity = 25f;
    [SerializeField] private float rayCastDistance = 100f;

    protected override void InitializeProjectile(Projectile rocket)
    {
        Ray ray = camera.ViewportPointToRay(new Vector3(0.5f, 0.5f));
        Vector3 target = Physics.Raycast(ray, out RaycastHit hit, rayCastDistance, projectileCollisionMask) ?
            hit.point :
            ray.origin + rayCastDistance * ray.direction;

        rocket.Rigidbody.position = projectileSource.position;
        Vector3 rocketDirection = (target - projectileSource.position).normalized;
        rocket.Rigidbody.rotation = Quaternion.LookRotation(rocketDirection) * Quaternion.Euler(90f, 0f, 0f);
        rocket.Rigidbody.velocity = rocketVelocity * rocketDirection;
    }
}
