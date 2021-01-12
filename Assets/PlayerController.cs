using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private CharacterController _characterController;
    public float _movementSpeed = 4.0f;
    public float _gravity = 0.25f;
    public float _jumpForce = 10f;
    public float mouseSensitivity = 2;
    private float yMovement;

    // Start is called before the first frame update
    void Start()
    {
        _characterController = GetComponent<CharacterController>();
    }


    void Update()
    {
        var xMovement = Input.GetAxis("Horizontal") * _movementSpeed;
        var zMovement = Input.GetAxis("Vertical") * _movementSpeed;

        //If the character is currently on the ground
        if (_characterController.isGrounded)
        {

            //And they are clicking the button to jump, then send the character up
            if (Input.GetKeyDown("space"))
            {
                yMovement = _jumpForce;
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
            yMovement -= _gravity;
        }

        Vector3 movement = transform.right * xMovement + transform.forward * zMovement + transform.up * yMovement;

        _characterController.Move(movement * Time.deltaTime);
    }
}
