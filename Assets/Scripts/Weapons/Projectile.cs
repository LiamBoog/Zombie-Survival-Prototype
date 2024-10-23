using System;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer), typeof(Rigidbody))]
public class Projectile : MonoBehaviour
{
    public Rigidbody Rigidbody => GetComponent<Rigidbody>();
    
    public event Action Impact;

    private void OnCollisionEnter(Collision _)
    {
        Impact?.Invoke();
    }
}
