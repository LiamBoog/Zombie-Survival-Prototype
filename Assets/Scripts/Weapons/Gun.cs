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
                projectile.CollisionMask = projectileCollisionMask;
                projectile.OnGet();
            },
            projectile =>
            {
                projectile.OnRelease();
            },
            Destroy
        );
        
        fire.action.Enable();
        fire.action.performed += InitializeProjectile;
    }

    private void OnDisable()
    {
        fire.action.Disable();
        fire.action.performed -= InitializeProjectile;
    }

    private void InitializeProjectile(InputAction.CallbackContext _)
    {
        initializer = () =>
        {
            Projectile projectile = projectiles.Get();
            InitializeProjectile(projectile);

            void OnImpact(Vector3 _) => ReleaseProjectile();
            projectile.Impact += OnImpact;
            projectile.Expired += ReleaseProjectile;

            initializer = null;
            
            void ReleaseProjectile()
            {
                projectiles.Release(projectile);
                projectile.Impact -= OnImpact;
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
