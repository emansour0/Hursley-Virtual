using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    public float InteractionDistance = 0.5f;
    public int InteractableLayer = 9;


    // Start is called before the first frame update
    void Start()
    {

    }

    void FixedUpdate()
    {
        int mask = 1 << InteractableLayer;

        RaycastHit hit;
        Ray forwardRay = new Ray(transform.position, transform.forward);

        if (Physics.Raycast(forwardRay, out hit, InteractionDistance, mask))
        {
            //If the object hit was a pop up and the user clicked the mouse (0 corresponds to the left button on the mouse)
            if(hit.collider.CompareTag("Pop Up") && Input.GetMouseButtonDown(0))
            {
                PopupController popUpController = hit.collider.GetComponentInParent<PopupController>();

                //If this pop up was declared as interactable when clicked, then open the link given
                if(popUpController.IsInteractable)
                {
                    Application.OpenURL(popUpController.InteractableLink);
                }
            }
        }
    }
}
