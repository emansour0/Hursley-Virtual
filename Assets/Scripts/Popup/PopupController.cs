using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class PopupController : MonoBehaviour, IInteractableBase
{
    public int PlayerLayer = 8; //Unity layer that the player is placed in

    protected float PlayerHeightOffset = 0.5f; //Offset along y axis so that pop ups angle slightly upwards towards the player head
    protected float HorizontalMargin = 0.2f;
    protected float VerticalMargin = 0.1f;
    protected float SpacingBetweenHeadingAndContent = 0.1f;

    protected int RotationSpeed = 3;
    protected int BackgroundExpansionSpeed = 2;

    public Transform Background;
    public Transform Heading;
    public GameObject Content;

    public TextMesh HeadingText;
    public TextMesh ContentText;

    protected Vector3 headingSize;
    protected Vector3 contentSize;

    //The size of the background when the player is at a distance and it only fits the heading
    protected Vector3 minBackgroundScale;

    public int FollowPlayerRadius = 5;
    public int ShowContentRadius = 2;

    //Controls whether a user can click on this pop up to make it show a link
    public bool OpensLink = false; //Can they?
    public string InteractableLink; //Link to show

    // Start is called before the first frame update
    protected virtual void Start()
    {
        //Gets the size in Unity dimensions of both 
        headingSize = HeadingText.GetComponent<MeshRenderer>().bounds.size;
        contentSize = ContentText.GetComponent<MeshRenderer>().bounds.size;

        scaleInitialBackground();

        //Place the content text in line with the heading but move it down so that the content fits below it
        Vector3 contentPos = Content.transform.position;
        contentPos.y = Heading.position.y - (contentSize.y * 0.5f + headingSize.y * 0.5f + SpacingBetweenHeadingAndContent);
        Content.transform.position = contentPos;
        Background.localPosition = new Vector3(0, -0.1f, 4);
    }

    public virtual bool OnHover(Transform t)
    {
        //Implementable by variants of this script and called by PlayerInteraction
        return OpensLink;
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        int layerMask = 1 << PlayerLayer;

        Collider[] visibiltySphere = Physics.OverlapSphere(transform.position, FollowPlayerRadius, layerMask);
        Collider[] contentSphere = Physics.OverlapSphere(transform.position, ShowContentRadius, layerMask);


        if (visibiltySphere.Length > 0) rotateToPlayer(visibiltySphere[0].transform);

        if (contentSphere.Length > 0) showContent();

        else
        {
            Background.localScale = Vector3.Slerp(Background.localScale, minBackgroundScale, BackgroundExpansionSpeed * Time.deltaTime);
            Background.localPosition = Vector3.Slerp(Background.localPosition, new Vector3(0, -0.1f, 4), BackgroundExpansionSpeed * Time.deltaTime);
            Content.SetActive(false);
        }
    }

    public void OnClick()
    {
#if !UNITY_EDITOR
        if (OpensLink)
        {
            Utils.openWindow(InteractableLink);
        }
#endif
    }

    //Rotates the text to face the direction of the player
    public void rotateToPlayer(Transform player)
    {
        transform.rotation = Quaternion.Slerp(transform.rotation, Utils.FindRotationToPlayer(transform, player, PlayerHeightOffset), RotationSpeed * Time.deltaTime);
    }

    //Scales the background to its initial size (only the Header shown)
    protected void scaleInitialBackground()
    {
        float width = contentSize.x < headingSize.x ? headingSize.x + HorizontalMargin : contentSize.x + HorizontalMargin;
        float height = headingSize.y + VerticalMargin * 0.5f;

        //Height is in the z axis because the plane is rotated, so z is the height and x is the width
        Background.localScale = new Vector3(width, 1, height);
        minBackgroundScale = Background.localScale;
    }

    //Expands the background and displays the content text
    protected void showContent()
    {
        Content.SetActive(true); //Display content text

        //Increase background height by ContentLineHeight for every line of text
        float height = contentSize.y + SpacingBetweenHeadingAndContent + VerticalMargin * 0.5f;

        Vector3 targetSize = minBackgroundScale;
        targetSize.z += height;

        Background.localScale = Vector3.Slerp(Background.localScale, targetSize, BackgroundExpansionSpeed * Time.deltaTime);

        //Repositions scaled box so that it looks like the background expands downwards
        Vector3 initialPosition = Heading.position;
        initialPosition.y -= height * 0.5f;
        initialPosition.z = Background.position.z;

        Background.position = Vector3.Slerp(Background.position, initialPosition, BackgroundExpansionSpeed * Time.deltaTime);
        Background.localPosition = new Vector3(0, -0.1f, Background.localPosition.z);
    }

    //Returns the number of lines in a Text Box
    private int getNumberOfLines(TextMesh text)
    {
        return text.text.Split('\n').Length;
    }
}
