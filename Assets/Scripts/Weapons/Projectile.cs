using System;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer), typeof(Rigidbody))]
public abstract class Projectile : MonoBehaviour
{
    public struct ImpactInfo
    {
        public Vector3 point;
        public Transform target;
    }
    
    [SerializeField] private float lifetime = 5f;
    
    public event Action<ImpactInfo> Impact;
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

    public virtual void Initialize()
    {
        remainingLife = lifetime;
    }
    public abstract void Deinitialize();

    private void Update()
    {
        remainingLife -= Time.deltaTime;

        if (remainingLife > 0f)
            return;
        
        Expire();
    }

    private void OnCollisionEnter(Collision collision)
    {
        Impact?.Invoke(new ImpactInfo
        {
            point = collision.GetContact(0).point,
            target = collision.transform
        });
    }

    protected void Expire()
    {
        Expired?.Invoke();
    }
}
