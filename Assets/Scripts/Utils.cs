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

    //Used to wait for a certain number of frames in a coroutine
    public static IEnumerator Frames(int frameCount)
    {
        while (frameCount > 0)
        {
            frameCount--;
            yield return null;
        }
    }

    [DllImport("__Internal")]
    public static extern void openWindow(string url);
}
