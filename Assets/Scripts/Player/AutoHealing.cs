using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Damageable))]
public class AutoHealing : MonoBehaviour
{
    [SerializeField] private float healPerSecond;
    [SerializeField] private float minHeal = 1f;

    private Damageable damageable;
    
    private void OnEnable()
    {
        damageable = GetComponent<Damageable>();
        StartCoroutine(HealingRoutine());
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }

    private IEnumerator HealingRoutine()
    {
        while (true)
        {
            damageable.Heal(minHeal);
            yield return new WaitForSeconds(minHeal / healPerSecond);
        }
    }
}
