using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AvatarController : MonoBehaviour
{
    public Camera Camera;
    public string ChatbotName;
    public GameObject Avatar;

    private GameObject avatarInstance;
    private Vector3 cameraCorner;

    // Start is called before the first frame update
    void Start()
    {
        ChatbotController.CloseChatbotEvent.AddListener(CloseChatbot);
        ChatbotController.OpenChatbotEvent.AddListener(OpenChatbot);
    }

    private void OpenChatbot(List<(string, ChatbotController.MessageType)> messageLog, string chatbotName)
    {
        cameraCorner = Camera.ViewportToWorldPoint(new Vector3(0, 1, 0.25f));
        avatarInstance = Instantiate(Avatar, cameraCorner, Quaternion.Euler(0, 0, 0));
    }

    private void CloseChatbot()
    {
        Destroy(avatarInstance);
    }
}
