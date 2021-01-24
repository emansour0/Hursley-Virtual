using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    public float InteractionDistance = 2;
    public int PopupLayer = 9;
    public int ButtonLayer = 10;

    public GameObject PopupTextPrefab;

    private GameObject popup;


    // Start is called before the first frame update
    void Start()
    {

    }

    void Update()
    {
        int popupMask = 1 << PopupLayer;
        int ButtonMask = 1 << ButtonLayer;

        Ray forwardRay = new Ray(transform.position, transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(forwardRay, out hit, InteractionDistance, popupMask))
        {
#if !UNITY_EDITOR
            //If the object hit was a pop up and the user clicked the mouse (0 corresponds to the left button on the mouse)
            if(hit.collider.CompareTag("Pop Up") && Input.GetMouseButtonDown(0))
            {
                PopupController popUpController = hit.collider.GetComponentInParent<PopupController>();

                //If this pop up was declared as interactable when clicked, then open the link given
                if(popUpController.IsInteractable)
                {
                    openWindow(popUpController.InteractableLink);
                }
            }
#endif
        }
        if (Physics.Raycast(forwardRay, out hit, InteractionDistance, ButtonMask))
        {
            ButtonController buttonController = hit.collider.GetComponent<ButtonController>();

            buttonController.OnHover(transform);

            if (Input.GetMouseButtonDown(0))
            {
                buttonController.OnClick();
            }
        }
        else
        {
            Destroy(popup);
        }
    }

    [DllImport("__Internal")]
    private static extern void openWindow(string url);
}
