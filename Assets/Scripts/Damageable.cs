using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Damageable : MonoBehaviour
{
    [SerializeField] private float health = 100f;

    private float damage;

    public event Action<float> HealthChanged; 
    public event Action Died;

    private float HealthPercentage => (health - damage) / health;

    private void OnEnable()
    {
        damage = 0f;
    }

    public void Damage(float damage)
    {
        this.damage += damage;
        HealthChanged?.Invoke(HealthPercentage);

        if (this.damage < health)
            return;
        
        Died?.Invoke();
    }

    public void Heal(float healing)
    {
        damage = Mathf.Max(0f, damage - healing);
        HealthChanged?.Invoke(HealthPercentage);
    }
}
