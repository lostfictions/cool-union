using UnityEngine;
using System.Collections;

public static class VectorExtensions
{
    public static Vector2 xy(this Vector3 vector)
    {
        return new Vector2(vector.x, vector.y);
    }

    public static Vector2 yz(this Vector3 vector)
    {
        return new Vector2(vector.y, vector.z);
    }

    public static Vector2 xz(this Vector3 vector)
    {
        return new Vector2(vector.x, vector.z);
    }

    public static Vector3 zyx(this Vector3 vector)
    {
        return new Vector3(vector.z, vector.y, vector.x);
    }

    public static Vector2 zy(this Vector3 vector)
    {
        return new Vector2(vector.z, vector.y);
    }

    public static Vector3 x0y(this Vector2 vector)
    {
        return new Vector3(vector.x, 0, vector.y);
    }

    public static Vector3 xy0(this Vector2 vector)
    {
        return new Vector3(vector.x, vector.y, 0);
    }
}
