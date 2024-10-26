using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Damageable : MonoBehaviour
{
    [SerializeField] private float health = 100f;

    private float damage;

    public event Action<float> Damaged; 
    public event Action Died;

    private void OnEnable()
    {
        damage = 0f;
    }

    public void Damage(float damage)
    {
        this.damage += damage;
        Damaged?.Invoke((health - this.damage) / health);

        if (this.damage < health)
            return;
        
        Died?.Invoke();
    }
}
