using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class ComponentUtility
{
    public static IEnumerable<T> GetComponentsInRadius<T>(Vector3 center, float radius, int layerMask = ~0) where T : Component
    {
        return Physics.OverlapSphere(center, radius, layerMask)
            .Select(c => c.GetComponent<T>())
            .Where(o => o != null);
    }
}
