using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class ChatbotView : MonoBehaviour
{
    private string activeChatbot;

    public Text WelcomeMessage;

    public Transform Content;
    public ScrollRect ScrollView;
    public GameObject OutgoingMessage;
    public GameObject IncomingMessage;
    public InputField MessageBox;

    public CanvasGroup ChatbotElements;

    public static UnityEvent<string, string> UserEnteredMessageEvent = new UnityEvent<string, string>();

    private void Start()
    {
        ChatbotController.CloseChatbotEvent.AddListener(CloseChatbot);
        ChatbotController.OpenChatbotEvent.AddListener(OpenChatbot);
        ChatbotController.NewMessageReceivedEvent.AddListener(AddMessageToUI);

        ShowChatbotUI(false);
    }

    private void OpenChatbot(List<(string, ChatbotController.MessageType)> messageLog, string chatbotName)
    {
        activeChatbot = chatbotName;
        GenerateWelcomeMessage();

        foreach ((string m, ChatbotController.MessageType type) in messageLog)
        {
            AddMessageToUI(m, type);
        }

        ShowChatbotUI(true);

        MessageBox.enabled = true;
        MessageBox.ActivateInputField();
    }

    private void CloseChatbot()
    {
        //Destroy all messages displayed in the messages pane before closing, so this panel can be reused
        for (int i = 1; i < Content.childCount; i++)
        {
            Destroy(Content.GetChild(i).gameObject);
        }

        //Makes UI disappear
        ShowChatbotUI(false);
        ResetMessageBox();
        MessageBox.DeactivateInputField();
        MessageBox.enabled = false; //This stops random key presses being sent to watson after closing the chatbot
    }

    public void SendChatbotMessage(string message)
    {
        if (message.Replace(" ", "").Length > 0)
        {
            AddMessageToUI(message, ChatbotController.MessageType.Outgoing);
            UserEnteredMessageEvent.Invoke(message, activeChatbot);
            ResetMessageBox();
        }

        MessageBox.ActivateInputField();
    }

    private void AddMessageToUI(string message, ChatbotController.MessageType messageType)
    {
        GameObject messageObj = messageType == ChatbotController.MessageType.Incoming ? IncomingMessage : OutgoingMessage;
        Text text = messageObj.transform.GetChild(1).GetComponentInChildren<Text>();
        text.text = message;

        Instantiate(messageObj, Content);

        ScrollToBottom();
    }

    private void ShowChatbotUI(bool active)
    {
        ChatbotElements.alpha = active ? 1 : 0;
    }

    private void ResetMessageBox()
    {
        MessageBox.text = "";
    }

    void ScrollToBottom()
    {
        ScrollView.velocity = new Vector2(0f, 1000f);
    }


    private void GenerateWelcomeMessage()
    {
        WelcomeMessage.text = $"You are now talking to {activeChatbot.Replace("_", " ")}, say hi!\nPress the ESC key at any time to exit";
    }
}
