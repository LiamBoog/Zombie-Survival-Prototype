using System;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer), typeof(Rigidbody))]
public class Projectile : MonoBehaviour
{
    [SerializeField] private float lifetime = 5f;
    
    public Rigidbody Rigidbody => GetComponent<Rigidbody>();
    
    public event Action Impact;
    public event Action Expired;

    private float remainingLife;

    private void OnEnable()
    {
        remainingLife = lifetime;
    }

    private void Update()
    {
        remainingLife -= Time.deltaTime;

        if (remainingLife > 0f)
            return;
        
        Expired?.Invoke();
    }

    private void OnCollisionEnter(Collision _)
    {
        Impact?.Invoke();
    }
}
