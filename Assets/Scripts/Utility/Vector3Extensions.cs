using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Vector3Extensions
{
    public static Vector3 VectorTripleProduct(this Vector3 vector, Vector3 direction)
    {
        return Vector3.Cross(Vector3.Cross(vector, direction), vector);
    }
}
