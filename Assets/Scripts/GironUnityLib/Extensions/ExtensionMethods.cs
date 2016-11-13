using UnityEngine;
using System.Collections;

public static class ExtensionMethods
{
    // Transform


    /// <summary>
    /// Make the local up vector face a point.
    /// </summary>
    /// <param name="trans">A transform</param>
    /// <param name="point">Point to face</param>
    public static void FaceUp(this Transform trans, Vector3 point)
    {
        trans.up = (point - trans.position).normalized;
    }


    /// <summary>
    /// Rotates the object so it faces a point.
    /// Y-axis is assumed to be the forward vector in 2D-space
    /// </summary>
    /// <param name="trans">Object's transform</param>
    /// <param name="point">Point to look at</param>
    public static void LookAt2D(this Transform trans, Vector3 point)
    {
        trans.up = (point - trans.position).normalized;
        Quaternion q = Quaternion.Euler(0f, 0f, trans.rotation.eulerAngles.z);
        trans.rotation = q;
    }


    /// <summary>
    /// Make the object look in a certain direction.
    /// Y-axis i assumed to be the forward vector in 2D-space
    /// </summary>
    /// <param name="trans">Object's transform</param>
    /// <param name="direction">Direction to look in</param>
    public static void LookInDirection2D(this Transform trans, Vector3 direction)
    {
        trans.up = direction;
        Quaternion q = Quaternion.Euler(0f, 0f, trans.rotation.eulerAngles.z); // Hack to avoid rotation on X and Y
        trans.rotation = q;
    }


    /// <summary>
    /// Rotate towards a target angle in 2D space. Rotation is based an angle around the transform's z-axis 
    /// </summary>
    /// <param name="trans">The transform</param>
    /// <param name="from">Start angle</param>
    /// <param name="to">Target angle</param>
    /// <param name="t">Amount of rotation</param>
    public static void RotateTowards2D(this Transform trans, float from, float to, float t)
    {
        float angle = Mathf.MoveTowardsAngle(from, to, t);
        Quaternion rot = Quaternion.Euler(0f, 0f, angle);
        trans.rotation = rot;

    }




    // Vector 2

    /// <summary>
    /// Rotate a Vector2 via rotation matrix
    /// </summary>
    /// <param name="v">Vector to rotate</param>
    /// <param name="a">Angle to rotate the vector</param>
    /// <returns></returns>
    public static Vector2 Rotate(this Vector2 v, float a)
    {
        return new Vector2(v.x * Mathf.Cos(a) + v.y * Mathf.Sin(a), -v.x * Mathf.Sin(a) + v.y * Mathf.Cos(a));
    }

    /// <summary>
    /// The directed angle between a vector and a target vector.
    /// Clockwise range 0 to 180.
    /// Counter clockwise range 0 to -180.
    /// </summary>
    /// <param name="v">Reference vector</param>
    /// <param name="target">Target vector</param>
    /// <returns>Directed angle between vectors</returns>
    public static float DirectedAngle(this Vector2 v, Vector2 target)
    {
        return -Mathf.DeltaAngle(Mathf.Atan2(v.y, v.x) * Mathf.Rad2Deg, Mathf.Atan2(target.y, target.x) * Mathf.Rad2Deg);
    }

    /// <summary>
    /// Detemine if two vectors are almost equal.
    /// </summary>
    /// <param name="v1">First vector</param>
    /// <param name="v2">Second vector</param>
    /// <param name="epsilon">Epsilon for margin of error</param>
    /// <returns></returns>
    public static bool AlmostEqual(this Vector2 v1, Vector2 v2, float margin)
    {
        return (Vector2.SqrMagnitude(v1 - v2) <= margin);
    }


    // Quaternion

    /// <summary>
    /// Rotate aound y-axis
    /// </summary>
    /// <param name="q">Current rotation</param>
    /// <param name="degrees">Degrees to rotate</param>
    /// <returns></returns>
    public static Quaternion Yaw(this Quaternion q, float degrees)
    {
        Quaternion roll = Quaternion.Euler(0f, degrees, 0f);
        return (roll * q);
    }

    /// <summary>
    /// Rotate aound x-axis
    /// </summary>
    /// <param name="q">Current rotation</param>
    /// <param name="degrees">Degrees to rotate</param>
    /// <returns></returns>
    public static Quaternion Pitch(this Quaternion q, float degrees)
    {
        Quaternion roll = Quaternion.Euler(degrees, 0f, 0f);
        return (roll * q);
    }

    /// <summary>
    /// Rotate aound z-axis
    /// </summary>
    /// <param name="q">Current rotation</param>
    /// <param name="degrees">Degrees to rotate</param>
    /// <returns></returns>
    public static Quaternion Roll(this Quaternion q, float degrees)
    {
        Quaternion roll = Quaternion.Euler(0f, 0f, degrees);
        return (roll * q);
    }
}

