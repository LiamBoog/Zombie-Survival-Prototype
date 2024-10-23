using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Pool;

public class Bazooka : MonoBehaviour
{
    [SerializeField] private Projectile rocketPrefab;
    [SerializeField] private InputActionReference fire;
    [SerializeField] private new Camera camera;

    [SerializeField] private Transform rocketSource;
    [SerializeField] private float rocketVelocity = 25f;

    private GameObject rocketParent;
    private ObjectPool<Projectile> rockets;

    private void OnEnable()
    {
        rocketParent = new GameObject("Rockets");
        rockets = new ObjectPool<Projectile>(
            () =>
            {
                Projectile rocket = Instantiate(rocketPrefab, rocketParent.transform);
                rocket.gameObject.SetActive(false);
                return rocket;
            },
            rocket => rocket.gameObject.SetActive(true),
            rocket => rocket.gameObject.SetActive(false),
            Destroy
            );
        
        fire.action.Enable();
        fire.action.performed += OnFire;
    }

    private void OnDisable()
    {
        fire.action.Disable();
        fire.action.performed -= OnFire;
    }

    private void OnFire(InputAction.CallbackContext _)
    {
        Ray ray = camera.ViewportPointToRay(new Vector3(0.5f, 0.5f));
        if (!Physics.Raycast(ray, out RaycastHit hit, 100f))
            return;

        Projectile rocket = rockets.Get();
        rocket.transform.position = rocketSource.position;
        Vector3 rocketDirection = (hit.point - rocketSource.position).normalized;
        rocket.transform.rotation = Quaternion.LookRotation(hit.point - rocketSource.position) * Quaternion.Euler(90f, 0f, 0f);
        rocket.Rigidbody.velocity = rocketVelocity * rocketDirection;
        
        rocket.Impact += OnImpact;

        void OnImpact()
        {
            rockets.Release(rocket);
            rocket.Impact -= OnImpact;
        }
    }
}
