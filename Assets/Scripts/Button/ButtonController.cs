using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class ButtonController : MonoBehaviour, IInteractableBase
{
    Animator anim;
    public Transform popupPosition;
    GameObject popup;
    public float PopupTime = 0.1f;
    public GameObject ButtonPopup;

    public bool OpensLink;
    public string InteractableLink;

    public string PopupHeading;
    public string PopupContent;

    private Coroutine PopupTimeout;

    public float TimeTillLinkOpens;
    private float openLinkDelay;

    void Start()
    {
        anim = GetComponent<Animator>();
        ButtonPopup.transform.GetChild(0).GetComponent<TextMesh>().text = PopupHeading;
        ButtonPopup.transform.GetChild(1).GetComponent<TextMesh>().text = PopupContent;
    }

    void Update()
    {
        //Workaround a coroutine bug in Unity 2020.2 which stops the previous code from opening a link after the animation has played
        if (openLinkDelay > 0)
        {
            openLinkDelay -= Time.deltaTime;

            if (openLinkDelay < 0)
            {
                openLinkDelay = 0;
#if !UNITY_EDITOR
                Utils.openWindow(InteractableLink);
#endif
            }
        }
    }

    public bool OnHover(Transform player)
    {
        if (popup == null)
        {
            //Create the popup and rotate it to face the player
            popup = Instantiate(ButtonPopup, popupPosition.position, Quaternion.Euler(new Vector3(0, 0, 0)));
            Quaternion rotationToPlayer = Utils.FindRotationToObject(transform, player);
            popup.transform.rotation = rotationToPlayer;
        }
        else
        {
            StopCoroutine(PopupTimeout);
        }

        PopupTimeout = StartCoroutine(DestroyPopup());

        return OpensLink;
    }

    public void OnClick()
    {
        if (OpensLink)
        {
            openLinkDelay = TimeTillLinkOpens;
        }
        anim.SetTrigger("button_pressed");
    }

    IEnumerator DestroyPopup()
    {
        TextMesh heading = popup.GetComponent<PopupController>().HeadingText;
        TextMesh content = popup.GetComponent<PopupController>().ContentText;

        for (int i = 255; i > 0; i--)
        {
            heading.color = new Color32((byte)(255 * heading.color.r), (byte)(255 * heading.color.g), (byte)(255 * heading.color.b), (byte)i);
            content.color = new Color32((byte)(255 * content.color.r), (byte)(255 * content.color.g), (byte)(255 * content.color.b), (byte)i);

            yield return new WaitForSeconds(PopupTime);
        }

        Destroy(popup);
    }
}
