using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    public float InteractionDistance = 2;
    public int PopupLayer = 9;
    public int ButtonLayer = 10;
    public int ChatbotLayer = 11;
    public CanvasGroup InteractionUI;

    //Stores a reference to a chatbot controller if it has been opened by clicking on it by this script
    //This is used so it can be closed again
    private ChatbotController openChatbotController;

    // Start is called before the first frame update
    void Start()
    {

    }

    void Update()
    {
        int popupMask = 1 << PopupLayer;
        int ButtonMask = 1 << ButtonLayer;
        int ChatbotMask = 1 << ChatbotLayer;


        Ray forwardRay = new Ray(transform.position, transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(forwardRay, out hit, InteractionDistance, popupMask))
        {
            PopupController popUpController = hit.collider.GetComponentInParent<PopupController>();
            popUpController.OnHover();

            if(popUpController.OpensLink) ShowInteractUI(true);

            //If the object hit was a pop up and the user clicked the mouse (0 corresponds to the left button on the mouse)
            if(hit.collider.CompareTag("Pop Up") && Input.GetMouseButtonDown(0))
            {
                //If this pop up was declared as interactable when clicked, then open the link given
                if(popUpController.OpensLink)
                {
                    popUpController.OnClick();
                }
            }
        }
        else if (Physics.Raycast(forwardRay, out hit, InteractionDistance, ButtonMask))
        {
            ButtonController buttonController = hit.collider.GetComponent<ButtonController>();
            ShowInteractUI(true);
            buttonController.OnHover(transform);

            if (Input.GetMouseButtonDown(0))
            {
                StartCoroutine(buttonController.OnClick());
            }
        }
        else if (Input.GetKeyDown(KeyCode.Escape) && openChatbotController != null)
        {
            openChatbotController.CloseChatbot();
            openChatbotController = null;
        }
        else if (Physics.Raycast(forwardRay, out hit, InteractionDistance, ChatbotMask))
        {
            ShowInteractUI(true);

            if (Input.GetMouseButtonDown(0) && openChatbotController == null)
            {
                openChatbotController = hit.collider.GetComponent<ChatbotController>();
                openChatbotController.OpenChatbot();
            }
        }
        else
        {
            ShowInteractUI(false);
        }
    }

    private void ShowInteractUI(bool activate)
    {
        if (InteractionUI.alpha == 0 && activate == true) InteractionUI.alpha = 1;

        if (InteractionUI.alpha == 1 && activate == false) InteractionUI.alpha = 0;
    }
}
