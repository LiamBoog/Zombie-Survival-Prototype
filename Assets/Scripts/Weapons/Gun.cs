using System;
using System.Collections;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Pool;

public abstract class Gun : MonoBehaviour
{
    [SerializeField] private Projectile projectilePrefab;
    [SerializeField] private InputActionReference fire;

    [SerializeField] protected Transform projectileSource;
    [SerializeField] protected LayerMask projectileCollisionMask;
    [SerializeField] private float minTravelDistance = 1f;

    private GameObject projectileParent;
    private ObjectPool<Projectile> projectilePool;
    private Action onFixedUpdate;

    private void OnEnable()
    {
        projectileParent = new GameObject($"{name} Projectiles");
        projectilePool = new ObjectPool<Projectile>(CreateProjectile, OnGetProjectile, OnReleaseProjectile, Destroy);
        
        fire.action.Enable();
        fire.action.performed += SpawnProjectile;
    }

    private void OnDisable()
    {
        fire.action.Disable();
        fire.action.performed -= SpawnProjectile;
    }

    private Projectile CreateProjectile()
    {
        Projectile output = Instantiate(projectilePrefab, projectileParent.transform);
        output.gameObject.SetActive(false);
        return output;
    }

    private void OnGetProjectile(Projectile projectile)
    {
        projectile.gameObject.SetActive(true);
        projectile.GetComponent<MeshRenderer>().enabled = false;
        projectile.transform.parent = projectileParent.transform;
        projectile.CollisionMask = projectileCollisionMask;
        projectile.Initialize();
    }

    private void OnReleaseProjectile(Projectile projectile)
    {
        projectile.gameObject.SetActive(false);
        Rigidbody rigidbody = projectile.GetComponent<Rigidbody>();
        rigidbody.isKinematic = false;
        rigidbody.velocity = Vector3.zero;
        projectile.Deinitialize();
    }
    
    private void SpawnProjectile(InputAction.CallbackContext _)
    {
        onFixedUpdate += () =>
        {
            Projectile projectile = projectilePool.Get();
            projectile.StartCoroutine(DelayVisibility());
            InitializeProjectile(projectile);
            projectile.Expired += ReleaseProjectile;

            void ReleaseProjectile()
            {
                projectilePool.Release(projectile);
                projectile.Expired -= ReleaseProjectile;
            }

            IEnumerator DelayVisibility()
            {
                yield return null;
                while (Vector3.Distance(projectileSource.position, projectile.transform.position) < minTravelDistance)
                {
                    yield return null;
                }

                projectile.GetComponent<MeshRenderer>().enabled = true;
            }
        };
    }

    protected abstract void InitializeProjectile(Projectile projectile);

    private void FixedUpdate()
    {
        onFixedUpdate?.Invoke();
        onFixedUpdate = null;
    }
}
