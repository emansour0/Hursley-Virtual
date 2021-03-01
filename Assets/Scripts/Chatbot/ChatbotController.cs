using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ChatbotController : MonoBehaviour
{
    public string ChatbotName;
    public string Url;
    public string AssistantId;
    public string ApiKey;
    public string Version;

    private Session SessionID = null;
    public enum MessageType { Incoming, Outgoing };
    private List<(string, MessageType)> MessageLog;

    public static UnityEvent<List<(string, MessageType)>, string> OpenChatbotEvent = new UnityEvent<List<(string, MessageType)>, string>();
    public static UnityEvent CloseChatbotEvent = new UnityEvent();
    public static UnityEvent<string, MessageType> NewMessageReceivedEvent = new UnityEvent<string, MessageType>();

    // Start is called before the first frame update
    void Start()
    {
        MessageLog = new List<(string, MessageType)>();
        ApiManager.WatsonSessionEvent.AddListener(OnSessionChange);
        ApiManager.WatsonMessageEvent.AddListener(OnMessageReceived);
        ChatbotView.UserEnteredMessageEvent.AddListener(UserSentChatMessage);

        StartCoroutine(ApiManager.CreateSession(Url, ApiKey, Version, AssistantId));
    }

    public void OpenChatbot()
    {
        if(SessionID == null) StartCoroutine(ApiManager.CreateSession(Url, ApiKey, Version, AssistantId));
        OpenChatbotEvent.Invoke(MessageLog, ChatbotName);
    }

    public void CloseChatbot()
    {
        StartCoroutine(ApiManager.DeleteSession(Url, ApiKey, Version, SessionID, AssistantId));
        CloseChatbotEvent.Invoke();
    }

    //TODO check if the messages are for this chatbot!!

    private void UserSentChatMessage(string message, string chatbot)
    {
        //If this is for another chatbot, ignore it
        if (ChatbotName != chatbot) return;

        MessageLog.Add((message, MessageType.Outgoing));

        StartCoroutine(SendApiMessage(message));
    }

    private IEnumerator SendApiMessage(string message)
    {
        //If the session didn't exist already 
        if (SessionID == null) StartCoroutine(ApiManager.CreateSession(Url, ApiKey, Version, AssistantId));

        //While the session ID is being processed, wait here until it exists
        while (SessionID == null) yield return null;

        StartCoroutine(ApiManager.SendMessage(Url, ApiKey, Version, SessionID, AssistantId, message));
    }

    private void OnSessionChange(ApiSessionMessage message)
    {
        //If this message has a different assistant ID, its for a different chatbot, so ignore it
        if (message.AssistantId != AssistantId) return;

        if (message.MessageType == ApiSessionMessage.Type.created)
        {
            if (message.Successful)
            {
                Debug.Log($"A new session was created for {ChatbotName} with session ID {message.Payload.session_id}");
                SessionID = message.Payload;
            }
            else Debug.LogError($"There was an error creating a session for chatbot {ChatbotName}");
        }

        if (message.MessageType == ApiSessionMessage.Type.deleted)
        {
            if (message.Successful)
            {
                SessionID = null;
                Debug.Log($"Session {message.Payload.session_id} was closed for chatbot {ChatbotName}");
            }
            else Debug.LogError($"There was an error deleting the session for chatbot {ChatbotName}");
        }
    }

    private void OnMessageReceived(ApiResponseMessage response)
    {
        //If the message session ID doesn't match the session ID currently on this chatbot, then ignore it, because it is for another session
        if (response.SessionId != SessionID) return;

        if (response.Successful)
        {
            string text;

            //If there was no response given, or Watson returned a suggestion message, then return an unsure response to the user
            if (response.Payload.output.generic.Length == 0 || response.Payload.output.generic[0].response_type == "suggestion")
                text = "I'm sorry, I'm not sure, could you ask me something else?";

            else text = response.Payload.output.generic[0].text; //Return message otherwise

            MessageLog.Add((text, MessageType.Incoming));
            NewMessageReceivedEvent.Invoke(text, MessageType.Incoming);
        }
        else
        {
            Debug.LogError($"Chatbot {ChatbotName} was not able to recieve a response to message to Watson with session {response.SessionId.session_id}, creating new session and resending");
            
            SessionID = null; //Reset session
            StartCoroutine(SendApiMessage(response.Message)); //This coroutine handles sending and recreation of session if it doesn't exist (which we made sure of by setting it to null)
        }
    }
}
