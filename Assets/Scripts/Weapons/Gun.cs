using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Pool;

public abstract class Gun : MonoBehaviour
{
    [SerializeField] private Projectile projectilePrefab;
    [SerializeField] private InputActionReference fire;

    [SerializeField] protected Transform projectileSource;
    [SerializeField] protected LayerMask projectileCollisionMask;

    private GameObject projectileParent;
    private ObjectPool<Projectile> projectiles;
    private Action initializer;

    private void OnEnable()
    {
        projectileParent = new GameObject($"{name} Projectiles");
        projectiles = new ObjectPool<Projectile>(
            () =>
            {
                Projectile projectile = Instantiate(projectilePrefab, projectileParent.transform);
                projectile.gameObject.SetActive(false);
                return projectile;
            },
            projectile =>
            {
                projectile.gameObject.SetActive(true);
                projectile.CollisionMask = projectileCollisionMask;
                projectile.Initialize();
            },
            projectile =>
            {
                projectile.gameObject.SetActive(false);
                Rigidbody rigidbody = projectile.GetComponent<Rigidbody>();
                rigidbody.isKinematic = false;
                rigidbody.velocity = Vector3.zero;
                projectile.Deinitialize();
            },
            Destroy
        );
        
        fire.action.Enable();
        fire.action.performed += SpawnProjectile;
    }

    private void OnDisable()
    {
        fire.action.Disable();
        fire.action.performed -= SpawnProjectile;
    }

    private void SpawnProjectile(InputAction.CallbackContext _)
    {
        initializer = () =>
        {
            Projectile projectile = projectiles.Get();
            InitializeProjectile(projectile);

            projectile.Expired += ReleaseProjectile;

            initializer = null;
            
            void ReleaseProjectile()
            {
                projectiles.Release(projectile);
                projectile.Expired -= ReleaseProjectile;
            }
        };
    }

    private void FixedUpdate()
    {
        initializer?.Invoke();
    }

    protected abstract void InitializeProjectile(Projectile projectile);
}
