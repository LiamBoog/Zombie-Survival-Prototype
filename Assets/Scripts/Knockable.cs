using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Rigidbody))]
public class Knockable : MonoBehaviour
{
    [SerializeField] private UnityEvent<Vector3> knockbackHandler;

    public void ApplyKnockback(Vector3 impulse)
    {
        knockbackHandler?.Invoke(impulse);
    }

    public void DefaultKnockbackHandler(Vector3 impulse)
    {
        GetComponent<Rigidbody>().AddForce(impulse, ForceMode.Impulse);
    }
}
