using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobeController : MonoBehaviour
{
    public GameObject marker;
    public LatLong[] LatLongCoordinates;
    public float Radius;

    // Start is called before the first frame update
    void Start()
    {
        DisplayLatLongOnGlobe();
    }

    void DisplayLatLongOnGlobe()
    {
        foreach(LatLong coordinate in LatLongCoordinates)
        {
            Instantiate(marker, transform);
            marker.transform.localPosition = LatLongToCartesian(coordinate.Latitude, coordinate.Longitude, Radius);
        }
    }

    //This equation takes angles in DEGREES, not radians
    Vector3 LatLongToCartesian(float latitude, float longitude, float radius)
    {
        //To use the spherical coordinate conversion to cartesian, we need to convert latitude longitude to spherical by making latitude a polar coordinate
        //For more information on why, see https://vvvv.org/blog/polar-spherical-and-geographic-coordinates
        float polar = latitude - 90;

        //Converts degrees to radians
        polar *= Mathf.PI / 180;
        longitude *= Mathf.PI / 180;

        //Conversion from spherical coordinates to cartesian coordinates
        float x = radius * Mathf.Sin(polar) * Mathf.Cos(longitude);
        float y = radius * Mathf.Sin(polar) * Mathf.Sin(longitude);
        float z = radius * Mathf.Cos(polar);


        //The values for y and z were swapped because Unity swaps around the orientation for y and z used in mathematical notation (and Blender, which also has z as the 'upward' axis)
        //Chose to use the mathematical orientation for the math to avoid confusion with the equations
        return new Vector3(x, z, y);
    }
}

[Serializable]
public class LatLong
{
    public float Latitude;
    public float Longitude;
}
