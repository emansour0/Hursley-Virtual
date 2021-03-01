using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public static class Utils
{
    public static Quaternion FindRotationToObject(Transform transform, Transform target)
    {
        Vector3 targetPosition = transform.position;

        Vector3 relativePos = targetPosition - target.position;

        //Multiplying Quaternions is equivalent to combining them
        //Here, an extra -90 degree rotation is applied on the X, so that the plane faces forward
        return Quaternion.LookRotation(relativePos) * Quaternion.Euler(-90, 0, 0);
    }

    public static Quaternion FindRotationToPlayer(Transform transform, Transform player, float playerHeightOffset)
    {
        Vector3 targetPosition = transform.position;
        targetPosition.y -= playerHeightOffset;

        Vector3 relativePos = targetPosition - player.position;

        //Multiplying Quaternions is equivalent to combining them
        //Here, an extra -90 degree rotation is applied on the X, so that the plane faces forward
        return Quaternion.LookRotation(relativePos) * Quaternion.Euler(-90, 0, 0);
    }

    public static (string, string) SeparateFirstSplit(this string source, char delimiter)
    {
        var i = source.IndexOf(delimiter);

        return i == -1 ? (source, null) : (source.Substring(0, i), source.Substring(i+1, source.Length - (i + 1)));
    }

    //Gets script attached to either children, parent or the gameobject itself
    public static T GetComponentAcrossGameobject<T>(GameObject g)
    {
        T output;

        if (g.TryGetComponent<T>(out output)) return output;

        output = g.GetComponentInParent<T>();

        if (output != null) return output;

        output = g.GetComponentInChildren<T>();

        return output;
    }

    [DllImport("__Internal")]
    public static extern void openWindow(string url);
}
