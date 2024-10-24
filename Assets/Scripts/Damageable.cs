using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Damageable : MonoBehaviour
{
    [SerializeField] private float health = 100f;

    public event Action Died;

    public void Damage(float damage)
    {
        health -= damage;

        if (health > 0f)
            return;
        
        Died?.Invoke();
    }
}
