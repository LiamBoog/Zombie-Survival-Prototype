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
    [SerializeField] protected LayerMask collisionMask;
    
    public event Action<ImpactInfo> Impact;
    public event Action Expired;

    private float remainingLife;
    
    public Rigidbody Rigidbody => GetComponent<Rigidbody>();

    public virtual void Initialize()
    {
        remainingLife = lifetime;
        Rigidbody.includeLayers = collisionMask;
        Rigidbody.excludeLayers = ~collisionMask;
        Impact += OnImpact;
    }

    public virtual void Deinitialize()
    {
        StopAllCoroutines();
        Impact -= OnImpact;
    }

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

    private void OnImpact(ImpactInfo _)
    {
        GetComponent<MeshRenderer>().enabled = true;
    }
}
