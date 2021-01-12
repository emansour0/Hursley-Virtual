using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopupController : MonoBehaviour
{
    public int PlayerLayer = 8; //Unity layer that the player is placed in

    public int VisibilityRadius = 5;
    public int ContentRadius = 2;

    public float PlayerHeightOffset = 0.5f; //Offset along y axis so that pop ups angle slightly upwards towards the player head

    public int RotationSpeed = 3;
    public int BackgroundExpansionSpeed = 2;

    public Transform Background;
    public Transform Heading;
    public GameObject Content;

    public TextMesh HeadingText;
    public TextMesh ContentText;

    //The number of characters which are displayable in a width of 1 unit
    public int CharactersPerWidth = 16;

    //Heights of a line of text in Unity units, used for scaling
    public float HeadingLineHeight = 0.1f;
    public float ContentLineHeight = 0.1f;

    //Margin between the heading and content
    public float SpacingBeforeContent = 0.1f;
    public float SpacingAroundHeader = 0.1f;

    //The size of the background when the player is at a distance and it only fits the heading
    private Vector3 minBackgroundScale;

    // Start is called before the first frame update
    void Start()
    {
        scaleInitialBackground();

        Vector3 contentPos = Content.transform.position;
        contentPos.y = Heading.position.y - (SpacingBeforeContent + HeadingLineHeight * getNumberOfLines(HeadingText));
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
            Background.position = Vector3.Slerp(Background.position, Heading.position, BackgroundExpansionSpeed * Time.deltaTime);
            Content.SetActive(false);
        }
    }

    //Rotates the text to face the direction of the player
    private void rotateToPlayer(Transform player)
    {
            Vector3 targetPosition = transform.position;
            targetPosition.y -= PlayerHeightOffset;

            Vector3 relativePos = targetPosition - player.position;

            //Multiplying Quaternions is equivalent to combining them
            //Here, an extra -90 degree rotation is applied on the X, so that the plane faces forward
            Quaternion rotationToPlayer = Quaternion.LookRotation(relativePos) * Quaternion.Euler(-90, 0, 0);

            transform.rotation = Quaternion.Slerp(transform.rotation, rotationToPlayer, RotationSpeed * Time.deltaTime);
    }

    //Scales the background to its initial size (only the Header shown)
    private void scaleInitialBackground()
    {
        float width = (float) HeadingText.text.Length / (float) CharactersPerWidth;
        float height = SpacingAroundHeader + getNumberOfLines(HeadingText) * (float) HeadingLineHeight;

        Background.localScale = new Vector3(width, 1, height);
        minBackgroundScale = Background.localScale;

        Background.position = Heading.position;
    }

    //Expands the background and displays the content text
    private void showContent()
    {
        Content.SetActive(true); //Display content text

        //Increase background height by ContentLineHeight for every line of text
        float height = (float) getNumberOfLines(ContentText) * (float) ContentLineHeight;

        Vector3 targetSize = minBackgroundScale;
        targetSize.z += height;

        Background.localScale = Vector3.Slerp(Background.localScale, targetSize, BackgroundExpansionSpeed * Time.deltaTime);

        //Repositions scaled box so that it looks like the background expands downwards
        Vector3 initialPosition = Heading.position;
        initialPosition.y -= height * 0.5f;

        Background.position = Vector3.Slerp(Background.position, initialPosition, BackgroundExpansionSpeed * Time.deltaTime);
    }

    //Returns the number of lines in a Text Box
    private int getNumberOfLines(TextMesh text)
    {
        return text.text.Split('\n').Length;
    }
}
