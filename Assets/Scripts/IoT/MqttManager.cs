using AOT;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public static class MqttManager
{
    private static Dictionary<string, ActiveMqttConnection> connections = new Dictionary<string, ActiveMqttConnection>();

    public static void ConnectMqttBroker(MqttBase callback, string connectionId, MqttConnection connection)
    {
        if(connectionId.Contains(":"))
        {
            Debug.LogError($"The connectionId '{connectionId}' contains the prohibited ':' character");
            return;
        }

        connectMqtt(connectionId, connection.Host, connection.Username, connection.Password, connection.Path, connection.Topic,
                    connection.UseSsl, connection.CleanSession, connection.Port, OnConnect, OnMessageReceived);

        connections.Add(connectionId, new ActiveMqttConnection { Callback=callback, Active=false, Connection=connection });
    }

    [DllImport("__Internal")]
    private static extern void connectMqtt(string connectionId, string host, string username, string password, 
        string path, string topic, bool useSsl, bool cleanSession, int port, Action<string> onConnectCallback, Action<string> onMessageCallback);

    [MonoPInvokeCallback(typeof(Action<string>))]
    private static void OnConnect(string connectionId)
    {
        Debug.Log($"Connected to MQTT connection {connectionId}");

        connections[connectionId].Active = true;
    }

    [MonoPInvokeCallback(typeof(Action<string>))]
    private static void OnMessageReceived(string message)
    {
        (string connectionId, string payload) = message.SeparateFirstSplit(':');
        (string topic, string content) = payload.SeparateFirstSplit(':');

        ActiveMqttConnection connection = connections[connectionId];
        connection.Callback.OnMqttMessageReceived(topic, content);
    }
}


public class ActiveMqttConnection
{
    public MqttBase Callback;
    public MqttConnection Connection;
    public bool Active;
}