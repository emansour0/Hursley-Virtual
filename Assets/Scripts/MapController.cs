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

        (float x, float y) = GetLocalPosition(50.7581f, -1.40646f);

        GameObject markerInst = Instantiate(marker, transform.position, Quaternion.Euler(0, 0, 0));
        markerInst.transform.Translate(x, y, 0.01f);
        markerInst.transform.parent = transform;

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
        FerryMessage data = JsonUtility.FromJson<MapController.FerryMessage>(message);

        (float x, float y) = GetLocalPosition(data.position[0], data.position[1]);
        Debug.Log(message);
        Debug.Log($"Latitude: {data.position[0]}  Longitude: {data.position[1]}");

        GameObject markerInst = Instantiate(marker, transform.position, Quaternion.Euler(new Vector3(0, 0, 0)));
        markerInst.transform.Translate(x, y, 0.01f);
        markerInst.transform.parent = transform;

        if(markers.ContainsKey(topic))
        {
            Destroy(markers[topic]);
            markers.Remove(topic);
        }

        markers.Add(topic, markerInst);


        GameObject popup = markers[topic].GetComponent<HoverAndShowController>().PopupTemplate;
        popup.transform.GetChild(0).GetComponent<TextMesh>().text = topic;
        popup.transform.GetChild(1).GetComponent<TextMesh>().text = $"Latitude: {data.position[0]}\nLongitude: {data.position[1]}";
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
