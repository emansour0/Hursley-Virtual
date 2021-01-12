using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopupController : MonoBehaviour
{
    public int PlayerLayer = 8;
    public int VisibilityRadius = 5;
    public int ContentRadius = 2;

    public float PlayerHeightOffset = 0.5f;
    public int RotationSpeed = 3;
    public int BackgroundExpansionSpeed = 2;

    public Transform Background;
    public Transform Heading;
    public GameObject Content;

    public TextMesh HeadingText;
    public TextMesh ContentText;

    public int CharactersPerWidth = 16;
    public float LineHeight = 0.1f;
    public float ContentLineHeight = 0.1f;

    private Vector3 minBackgroundScale;

    // Start is called before the first frame update
    void Start()
    {
        scaleBackground();
        Vector3 contentPos = Content.transform.position;
        contentPos.y = Heading.position.y - (0.1f + LineHeight * getNumberOfLines(HeadingText));
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

    private void rotateToPlayer(Transform player)
    {
            Vector3 targetPosition = transform.position;
            targetPosition.y -= PlayerHeightOffset;

            Vector3 relativePos = targetPosition - player.position;

            Quaternion rotationToPlayer = Quaternion.LookRotation(relativePos) * Quaternion.Euler(-90, 0, 0);

            transform.rotation = Quaternion.Slerp(transform.rotation, rotationToPlayer, RotationSpeed * Time.deltaTime);
    }

    //Scales the background to its initial size (only the Header shown)
    private void scaleBackground()
    {
        float width = (float) HeadingText.text.Length / (float) CharactersPerWidth;
        float height = (float) 0.3 + getNumberOfLines(HeadingText) * (float) LineHeight;

        Background.localScale = new Vector3(width, 1, height);
        minBackgroundScale = Background.localScale;

        Background.position = Heading.position;
    }

    private void showContent()
    {
        Content.SetActive(true);

        float height = (float) getNumberOfLines(ContentText) * (float) ContentLineHeight;

        Vector3 targetSize = minBackgroundScale;
        targetSize.z += height;

        Background.localScale = Vector3.Slerp(Background.localScale, targetSize, BackgroundExpansionSpeed * Time.deltaTime);

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
