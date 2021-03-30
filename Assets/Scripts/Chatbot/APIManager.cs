using System;
using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;

public static class ApiManager
{
    public enum RequestType { Post, Delete };

    public class ApiSessionEvent : UnityEvent<ApiSessionMessage> { }
    public class ApiMessageEvent : UnityEvent<ApiResponseMessage> { }

    public static ApiSessionEvent WatsonSessionEvent = new ApiSessionEvent();
    public static ApiMessageEvent WatsonMessageEvent = new ApiMessageEvent();


    public static UnityWebRequest GetWebRequest(RequestType requestType, string endpoint, string version, string apikey, string payload)
    {
        if (requestType == RequestType.Post)
        {
            return AddApiHeaders(UnityWebRequest.Post($"{endpoint}?version={version}", payload), apikey);
        }
        else
        {
            return AddApiHeaders(UnityWebRequest.Delete($"{endpoint}?version={version}"), apikey);
        }
    }

    public static UnityWebRequest AddApiHeaders(UnityWebRequest webRequest, string ApiKey)
    {
        string encoding = "ISO-8859-1";
        string api_auth = $"apikey:{ApiKey}";
        string api_auth_bytes = Convert.ToBase64String(System.Text.Encoding.GetEncoding(encoding).GetBytes(api_auth));

        webRequest.SetRequestHeader("Content-Type", "application/json");
        webRequest.SetRequestHeader("Authorization", "Basic " + api_auth_bytes);

        return webRequest;
    }

    //TODO Version config file
    public static IEnumerator CreateSession(string url, string apikey, string version, string assistantId)
    {
        using (UnityWebRequest webRequest = GetWebRequest(RequestType.Post, $"{url}/v2/assistants/{assistantId}/sessions", version, apikey, ""))
        {
            yield return webRequest.SendWebRequest();

            if (webRequest.result == UnityWebRequest.Result.ProtocolError || webRequest.result == UnityWebRequest.Result.ConnectionError)
            {
                WatsonSessionEvent.Invoke(new ApiSessionMessage(assistantId, ApiSessionMessage.Type.created, false));
                yield break;
            }

            Session sessionMessage = JsonUtility.FromJson<Session>(webRequest.downloadHandler.text);
            WatsonSessionEvent.Invoke(new ApiSessionMessage(assistantId, ApiSessionMessage.Type.created, true, sessionMessage));
        }
    }

    public static IEnumerator DeleteSession(string url, string apikey, string version, Session sessionId, string assistantId)
    {
        using (UnityWebRequest webRequest = GetWebRequest(RequestType.Delete, $"{url}/v2/assistants/{assistantId}/sessions/{sessionId.session_id}", version, apikey, ""))
        {
            yield return webRequest.SendWebRequest();

            if (webRequest.result == UnityWebRequest.Result.ProtocolError || webRequest.result == UnityWebRequest.Result.ConnectionError)
            {
                Debug.Log($"{url}/v2/assistants/{assistantId}/sessions/{sessionId.session_id}");
                Debug.Log(webRequest.error);
                WatsonSessionEvent.Invoke(new ApiSessionMessage(assistantId, ApiSessionMessage.Type.deleted, false, sessionId));
                yield break;
            }
            WatsonSessionEvent.Invoke(new ApiSessionMessage(assistantId, ApiSessionMessage.Type.deleted, true, sessionId));
        }
    }

    public static IEnumerator SendMessage(string url, string apikey, string version, Session sessionId, string assistantId, string message)
    {
        string payload = $"{{\"input\": {{\"text\": \"{message}\"}}}}";
        byte[] bodyRaw = Encoding.UTF8.GetBytes(payload);

        using (UnityWebRequest webRequest = GetWebRequest(RequestType.Post, $"{url}/v2/assistants/{assistantId}/sessions/{sessionId.session_id}/message", version, apikey, payload))
        {
            webRequest.uploadHandler = new UploadHandlerRaw(bodyRaw);
            webRequest.downloadHandler = new DownloadHandlerBuffer();

            yield return webRequest.SendWebRequest();

            if (webRequest.result == UnityWebRequest.Result.ProtocolError || webRequest.result == UnityWebRequest.Result.ConnectionError)
            {
                WatsonMessageEvent.Invoke(new ApiResponseMessage(false, sessionId, message));
                yield break;
            }
            Message responseMessage = JsonUtility.FromJson<Message>(webRequest.downloadHandler.text);
            WatsonMessageEvent.Invoke(new ApiResponseMessage(true, responseMessage, sessionId, message));
        }
    }
}

public class ApiMessage
{
    public bool Successful { get; protected set; }
}

public class ApiSessionMessage : ApiMessage
{
    public enum Type { created, deleted }

    public Session Payload { get; private set; }
    public Type MessageType;
    public string AssistantId;

    public ApiSessionMessage(string assistantId, ApiSessionMessage.Type messageType, bool successful, Session payload)
    {
        AssistantId = assistantId;
        Successful = successful;
        Payload = payload;
        MessageType = messageType;
    }

    public ApiSessionMessage(string assistantId, ApiSessionMessage.Type messageType, bool successful)
    {
        AssistantId = assistantId;
        Successful = successful;
        MessageType = messageType;
    }
}

public class ApiResponseMessage : ApiMessage
{
    public string Message { get; private set; }
    public Message Payload { get; private set; }
    public Session SessionId;

    public ApiResponseMessage(bool successful, Message payload, Session sessionId, string originalMessage)
    {
        Payload = payload;
        Successful = successful;
        SessionId = sessionId;
        Message = originalMessage;
    }

    public ApiResponseMessage(bool successful, Session sessionId, string originalMessage)
    {
        Successful = successful;
        SessionId = sessionId;
        Message = originalMessage;
    }
}

