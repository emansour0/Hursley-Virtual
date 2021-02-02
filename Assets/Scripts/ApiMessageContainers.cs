using System;

[Serializable]
public class Session
{
    public string session_id;
}

[Serializable]
public class Message
{
    public Output output;
}

[Serializable]
public class Output
{
    public Intent[] intents;
    public Entity[] entities;
    public Generic[] generic;
}

[Serializable]
public class Intent
{
    public string intent;
    public float confidence;
}

[Serializable]
public class Entity
{
    public string entity;
    public int[] location;
    public string value;
    public int confidence;
}

[Serializable]
public class Generic
{
    public string response_type;
    public string text;
}
