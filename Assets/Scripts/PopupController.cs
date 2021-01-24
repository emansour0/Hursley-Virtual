using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopupController : MonoBehaviour
{
    public int PlayerLayer = 8; //Unity layer that the player is placed in

    public int VisibilityRadius = 5;
    public int ContentRadius = 2;

    public float PlayerHeightOffset = 0.5f; //Offset along y axis so that pop ups angle slightly upwards towards the player head
    public float HorizontalMargin = 0.2f;
    public float VerticalMargin = 0.1f;
    public float SpacingBetweenHeadingAndContent = 0.1f;

    public int RotationSpeed = 3;
    public int BackgroundExpansionSpeed = 2;

    public Transform Background;
    public Transform Heading;
    public GameObject Content;

    public TextMesh HeadingText;
    public TextMesh ContentText;

    private Vector3 headingSize;
    private Vector3 contentSize;

    //The size of the background when the player is at a distance and it only fits the heading
    private Vector3 minBackgroundScale;

    //Controls whether a user can click on this pop up to make it show a link
    public bool IsInteractable = false; //Can they?
    public string InteractableLink; //Link to show

    // Start is called before the first frame update
    void Start()
    {
        //Gets the size in Unity dimensions of both 
        headingSize = HeadingText.GetComponent<MeshRenderer>().bounds.size;
        contentSize = ContentText.GetComponent<MeshRenderer>().bounds.size;

        scaleInitialBackground();

        //Place the content text in line with the heading but move it down so that the content fits below it
        Vector3 contentPos = Content.transform.position;
        contentPos.y = Heading.position.y - (contentSize.y * 0.5f + headingSize.y * 0.5f + SpacingBetweenHeadingAndContent);
        Content.transform.position = contentPos;
    }

    // Update is called once per frame
    void Update()
    {
        int layerMask = 1 << PlayerLayer;

        Collider[] visibiltySphere = Physics.OverlapSphere(transform.position, VisibilityRadius, layerMask);
        Collider[] contentSphere = Physics.OverlapSphere(transform.position, ContentRadius, layerMask);


        if (visibiltySphere.Length > 0) rotateToPlayer(visibiltySphere[0].transform);

        if (contentSphere.Length > 0) showContent();

        else
        {
            Background.localScale = Vector3.Slerp(Background.localScale, minBackgroundScale, BackgroundExpansionSpeed * Time.deltaTime);
            Vector3 originalPosition = Heading.position;
            originalPosition.z = Background.position.z;
            Background.position = Vector3.Slerp(Background.position, originalPosition, BackgroundExpansionSpeed * Time.deltaTime);
            Content.SetActive(false);
        }
    }

    //Rotates the text to face the direction of the player
    public void rotateToPlayer(Transform player)
    {
        transform.rotation = Quaternion.Slerp(transform.rotation, findRotationToPlayer(player), RotationSpeed * Time.deltaTime);
    }

    public Quaternion findRotationToPlayer(Transform player)
    {
        Vector3 targetPosition = transform.position;
        targetPosition.y -= PlayerHeightOffset;

        Vector3 relativePos = targetPosition - player.position;

        //Multiplying Quaternions is equivalent to combining them
        //Here, an extra -90 degree rotation is applied on the X, so that the plane faces forward
        return Quaternion.LookRotation(relativePos) * Quaternion.Euler(-90, 0, 0);
    }

    //Scales the background to its initial size (only the Header shown)
    private void scaleInitialBackground()
    {
        float width = contentSize.x < headingSize.x ? headingSize.x + HorizontalMargin : contentSize.x + HorizontalMargin;
        float height = headingSize.y + VerticalMargin * 0.5f;

        //Height is in the z axis because the plane is rotated, so z is the height and x is the width
        Background.localScale = new Vector3(width, 1, height);
        minBackgroundScale = Background.localScale;

        Background.position = new Vector3(Heading.position.x, Heading.position.y, Background.position.z);
    }

    //Expands the background and displays the content text
    private void showContent()
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
    }

    //Returns the number of lines in a Text Box
    private int getNumberOfLines(TextMesh text)
    {
        return text.text.Split('\n').Length;
    }
}
