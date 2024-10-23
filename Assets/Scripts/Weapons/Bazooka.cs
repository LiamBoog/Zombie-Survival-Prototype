using UnityEngine;

public class Bazooka : Gun
{
    [SerializeField] private new Camera camera;

    [SerializeField] private float rocketVelocity = 25f;

    protected override void InitializeProjectile(Projectile rocket)
    {
        Ray ray = camera.ViewportPointToRay(new Vector3(0.5f, 0.5f));
        Vector3 target;
        if (Physics.Raycast(ray, out RaycastHit hit, 100f))
        {
            target = hit.point;
        }
        else
        {
            target = ray.origin + 100f * ray.direction;
        }

        rocket.Rigidbody.position = projectileSource.position;
        Vector3 rocketDirection = (target - projectileSource.position).normalized;
        rocket.transform.rotation = Quaternion.LookRotation(rocketDirection) * Quaternion.Euler(90f, 0f, 0f);
        rocket.Rigidbody.velocity = rocketVelocity * rocketDirection;
    }
}
