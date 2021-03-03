using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoverAndShowController : MonoBehaviour, IInteractableBase
{
    public string Heading;
    public string Content;

    public GameObject PopupTemplate;
    public Transform PopupPosition;
    GameObject popup;
    public float PopupDecay = 0;
    private Coroutine PopupTimeout;

    //Call this to display 
    public bool OnHover(Transform player)
    {
        if (popup == null)
        {
            //Create the popup and rotate it to face the player
            popup = Instantiate(PopupTemplate, PopupPosition.position, Quaternion.Euler(new Vector3(0, 0, 0)));

            popup.transform.GetChild(0).GetComponent<TextMesh>().text = Heading;
            popup.transform.GetChild(1).GetComponent<TextMesh>().text = Content;

            Quaternion rotationToPlayer = Utils.FindRotationToObject(transform, player);
            popup.transform.rotation = rotationToPlayer;
        }
        else
        {
            StopCoroutine(PopupTimeout);
        }

        PopupTimeout = StartCoroutine(DestroyPopup());

        return false;
    }

    IEnumerator DestroyPopup()
    {
        TextMesh heading = popup.GetComponent<PopupController>().HeadingText;
        TextMesh content = popup.GetComponent<PopupController>().ContentText;
        

        for (int i = 255; i > 0; i--)
        {
            heading.color = new Color32((byte)(255 * heading.color.r), (byte)(255 * heading.color.g), (byte)(255 * heading.color.b), (byte)i);
            content.color = new Color32((byte)(255 * content.color.r), (byte)(255 * content.color.g), (byte)(255 * content.color.b), (byte)i);

            yield return new WaitForSeconds(PopupDecay);

            if (PopupDecay == 0) break;
        }

        Destroy(popup);
    }

    public void OnClick()
    {
        //Do nothing, just hover
    }
}
