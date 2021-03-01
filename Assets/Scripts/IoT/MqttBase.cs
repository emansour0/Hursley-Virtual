using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MqttBase : MonoBehaviour
{
    public string ConnectionId;
    public MqttConnection MqttConnection;

    public abstract void OnMqttMessageReceived(string topic, string message);
}

[Serializable]
public class MqttConnection
{
    public string Host;
    public string Username;
    public string Password;
    public string Path;
    public string Topic;
    public int Port;
    public bool UseSsl;
    public bool CleanSession;
}
