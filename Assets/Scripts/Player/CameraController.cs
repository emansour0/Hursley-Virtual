using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float MouseSensitivity = 3;
    public float SmoothingSpeed = 2;
    private float xRotation = 0;
    private float yRotation = 0;
    private float yTargetRotation = 0;

    private bool frozen;

    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;

        ChatbotController.OpenChatbotEvent.AddListener(FreezeMovement);
        ChatbotController.CloseChatbotEvent.AddListener(UnFreezeMovement);
    }

    private void FreezeMovement(List<(string, ChatbotController.MessageType)> arg0, string arg1)
    {
        frozen = true;

        Cursor.lockState = CursorLockMode.Confined;
    }

    private void FreezeMovement()
    {
        frozen = true;

        Cursor.lockState = CursorLockMode.Confined;
    }

    public void UnFreezeMovement()
    {
        frozen = false;

        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        if (frozen) return;

        float mouseX = Input.GetAxis("Mouse X") * MouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * MouseSensitivity;

        yTargetRotation = Mathf.Clamp(yTargetRotation - mouseY, -90, 90);

        xRotation = Mathf.Lerp(xRotation, mouseX, SmoothingSpeed * Time.deltaTime);
        yRotation = Mathf.Lerp(yRotation, yTargetRotation, SmoothingSpeed * Time.deltaTime);


        transform.localRotation = Quaternion.Euler(transform.localRotation.y + yRotation, 0, 0);
        transform.parent.transform.Rotate(Vector3.up * xRotation);
    }
}
