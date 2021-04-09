using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float MouseSensitivity = 1;
    public float SmoothingTime = 0.1f;
    private float xSmoothingSpeed = 0;
    private float ySmoothingSpeed = 0;
    private float xRotation = 180;
    private float yRotation = 0;
    private float yTargetRotation = 0;
    private float xTargetRotation = 180;

    private bool frozen;

    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;

        ChatbotController.OpenChatbotEvent.AddListener(FreezeMovement);
        ChatbotController.CloseChatbotEvent.AddListener(UnFreezeMovement);
        PauseMenu.FreezeCameraEvent.AddListener(FreezeMovement);
        PauseMenu.ResumeCameraEvent.AddListener(UnFreezeMovement);
    }

    //Used to freeze the player, currently called when they click on a chatbot to stop their screen from moving
    private void FreezeMovement(List<(string, ChatbotController.MessageType)> arg0, string arg1)
    {
        frozen = true;

        Cursor.lockState = CursorLockMode.Confined;
    }

    // Used to freeze the player, used when pausing the game
    public void FreezeMovement()
    {
        frozen = true;

        Cursor.lockState = CursorLockMode.None;
    }

    //Removes the effects of the FreezeMovement() method
    public void UnFreezeMovement()
    {
        frozen = false;

        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        //If the frozen bool is set, we don't want the camera to move, so ignore all code in the update
        if (frozen) return;

        float mouseX = Input.GetAxis("Mouse X") * MouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * MouseSensitivity;

        yTargetRotation = Mathf.Clamp(yTargetRotation - mouseY, -90, 90);
        xTargetRotation = xTargetRotation + mouseX;

        xRotation = Mathf.SmoothDamp(xRotation, xTargetRotation, ref xSmoothingSpeed, SmoothingTime);
        yRotation = Mathf.SmoothDamp(yRotation, yTargetRotation, ref ySmoothingSpeed, SmoothingTime);

        transform.localRotation = Quaternion.Euler(transform.localRotation.y + yRotation, 0, 0);
        transform.parent.rotation = Quaternion.Euler(0, transform.parent.rotation.y + xRotation, 0);    
    }
}
