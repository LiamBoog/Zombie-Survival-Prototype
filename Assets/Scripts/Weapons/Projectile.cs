using System;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer), typeof(Rigidbody))]
public abstract class Projectile : MonoBehaviour
{
    [SerializeField] private float lifetime = 5f;
    
    public event Action<Vector3> Impact;
    public event Action Expired;

    private float remainingLife;
    private LayerMask targetMask;
    
    public Rigidbody Rigidbody => GetComponent<Rigidbody>();

    public LayerMask CollisionMask
    {
        set
        {
            Rigidbody rigidbody = GetComponent<Rigidbody>();
            rigidbody.includeLayers = value;
            rigidbody.excludeLayers = ~value;
        }
    }

    protected abstract void Initialize();
    protected abstract void Deinitialize();
    
    public void OnGet()
    {
        gameObject.SetActive(true);
        remainingLife = lifetime;
        
        Initialize();
    }

    public void OnRelease()
    {
        gameObject.SetActive(false);
        GetComponent<Rigidbody>().velocity = Vector3.zero;
        
        Deinitialize();
    }
    
    private void Update()
    {
        remainingLife -= Time.deltaTime;

        if (remainingLife > 0f)
            return;
        
        Expired?.Invoke();
    }

    private void OnCollisionEnter(Collision collision)
    {
        Impact?.Invoke(collision.GetContact(0).point);
    }
}
