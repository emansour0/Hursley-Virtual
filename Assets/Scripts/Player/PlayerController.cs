using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private CharacterController _characterController;
    public float MovementSpeed = 4.0f;
    public float Gravity = 0.25f;
    public float JumpForce = 10f;
    public float MouseSensitivity = 2;
    private float yMovement;

    private float temporaryMovementSpeed;

    // Start is called before the first frame update
    void Start()
    {
        temporaryMovementSpeed = MovementSpeed;

        _characterController = GetComponent<CharacterController>();
        ChatbotController.OpenChatbotEvent.AddListener(FreezePlayer);
        ChatbotController.CloseChatbotEvent.AddListener(UnfreezePlayer);
    }

    private void FreezePlayer(List<(string, ChatbotController.MessageType)> arg0, string arg1)
    {
        temporaryMovementSpeed = MovementSpeed;
        MovementSpeed = 0;
    }

    private void UnfreezePlayer()
    {
        MovementSpeed = temporaryMovementSpeed;
    }


    void Update()
    {
        var xMovement = Input.GetAxis("Horizontal") * MovementSpeed;
        var zMovement = Input.GetAxis("Vertical") * MovementSpeed;

        //If the character is currently on the ground
        if (_characterController.isGrounded)
        {

            //And they are clicking the button to jump, then send the character up
            if (Input.GetKeyDown("space") && MovementSpeed > 0)
            {
                yMovement = JumpForce;
            }

            //If not, keep them at the same level on the ground
            else
            {
                yMovement = -2;
            }
        }
        //If the character is not on the ground (in the air), then deccelerate their upwards speed
        else
        {
            yMovement -= Gravity * Time.deltaTime;
        }

        Vector3 movement = transform.right * xMovement + transform.forward * zMovement + transform.up * yMovement;

        _characterController.Move(movement * Time.deltaTime);
    }
}
