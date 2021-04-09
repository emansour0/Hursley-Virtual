using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    public float InteractionDistance = 2;
    public int InteractablesLayer = 9;
    public int ChatbotLayer = 11;
    public CanvasGroup InteractionUI;

    //Stores a reference to a chatbot controller if it has been opened by clicking on it by this script
    //This is used so it can be closed again
    private ChatbotController openChatbotController;

    void Update()
    {
        int InteractablesMask = 1 << InteractablesLayer;
        int ChatbotMask = 1 << ChatbotLayer;


        Ray forwardRay = new Ray(transform.position, transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(forwardRay, out hit, InteractionDistance, InteractablesMask))
        {
            IInteractableBase interactable = Utils.GetComponentAcrossGameobject<IInteractableBase>(hit.collider.gameObject);

            bool interact = interactable.OnHover(transform);

            if (interact) ShowInteractUI(true);

            if(Input.GetMouseButtonDown(0) && !PauseMenu.isPaused)
            {
                interactable.OnClick();
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
