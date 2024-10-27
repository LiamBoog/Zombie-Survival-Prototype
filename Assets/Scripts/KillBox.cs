using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(BoxCollider))]
public class KillBox : MonoBehaviour
{
    private void OnCollisionEnter(Collision other)
    {
        if (!other.gameObject.TryGetComponent(out Damageable damageable))
            return;
        
        damageable.Damage(float.MaxValue);
    }
}
