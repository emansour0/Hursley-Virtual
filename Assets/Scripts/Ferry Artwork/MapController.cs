using AOT;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class MapController : MqttBase
{
    public GameObject marker;

    private Dictionary<string, GameObject> markers = new Dictionary<string, GameObject>();

    public float UpperLatitudeBound;
    public float LowerLatitudeBound;
    public float LeftLongitudeBound;
    public float RightLongitudeBound;

    //These are 1 on the painting (I just know this as a fact and remembered)
    public float width;
    public float height;

    private float latitudeCentre;
    private float longitudeCentre;

    private float latitudeRange;
    private float longitudeRange;

    // Start is called before the first frame update
    void Start()
    {
        //Calculations needed to calibrate the latitude and longitude to the board
        latitudeCentre = (UpperLatitudeBound + LowerLatitudeBound) / 2;
        longitudeCentre = (RightLongitudeBound + LeftLongitudeBound) / 2;
        latitudeRange = UpperLatitudeBound - LowerLatitudeBound;
        longitudeRange = RightLongitudeBound - LeftLongitudeBound;

#if !UNITY_EDITOR
        if(MqttConnection !=null && ConnectionId != null)
            MqttManager.ConnectMqttBroker(this, ConnectionId, MqttConnection);
#endif
    }

    //Returns the position in Unity space on the artwork when latitude and longitude are input
    private (float, float) GetLocalPosition(float latitude, float longitude)
    {
        float longitudePerWidth = width / longitudeRange;
        float latitudePerHeight = height / latitudeRange;

        float xPos = (longitude - longitudeCentre) * longitudePerWidth;
        float yPos = (latitude - latitudeCentre) * latitudePerHeight;

        return (-xPos, yPos);
    }

    public override void OnMqttMessageReceived(string topic, string message)
    {
        if (topic.Contains("test")) return; //ignore the test message

        FerryMessage data = JsonUtility.FromJson<MapController.FerryMessage>(message);

        float latitude = data.position[0];
        float longitude = data.position[1];

        if (!IsOnMap(latitude, longitude)) return; //if the point isnt on the map, ignore it
        
        (float x, float y) = GetLocalPosition(latitude, longitude);

        GameObject markerInst = Instantiate(marker, transform.position, Quaternion.Euler(new Vector3(0, 0, 0)));
        markerInst.transform.Translate(x, y, 0.01f);
        markerInst.transform.parent = transform;

        if (markers.ContainsKey(topic))
        {
            Destroy(markers[topic]);
            markers.Remove(topic);
        }

        markers.Add(topic, markerInst);

        HoverAndShowController markerController = markers[topic].GetComponent<HoverAndShowController>();
        GameObject popup = markerController.PopupTemplate;
        markerController.PopupDecay = 0;
        markerController.Heading = topic.Split('/')[topic.Split('/').Length - 1];
        markerController.Content = $"Latitude: {data.position[0]}\nLongitude: {data.position[1]}";
    }

    private bool IsOnMap(float latitude, float longitude)
    {
        if (latitude < LowerLatitudeBound || latitude > UpperLatitudeBound) return false;

        if (longitude < LeftLongitudeBound || longitude > RightLongitudeBound) return false;

        return true;
    }

    [Serializable]
    class FerryMessage
    {
        public float[] position;
        public int direction;
        public float speed;
        public int time;
        public int timestamp;
    }
}
