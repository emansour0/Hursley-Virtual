using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobeLabelController : PopupController
{
    private bool hovered;

    protected override void Start()
    {
        VerticalMargin = 0.01f;
        HorizontalMargin = 0.01f;
        PlayerHeightOffset = 1;
        SpacingBetweenHeadingAndContent = 0.01f;
        BackgroundExpansionSpeed = 4;
        base.Start();
    }

    // Update is called once per frame
    protected override void Update()
    {
        int layerMask = 1 << PlayerLayer;
        Collider[] visibiltySphere = Physics.OverlapSphere(transform.position, FollowPlayerRadius, layerMask);

        if (visibiltySphere.Length > 0) rotateToPlayer(visibiltySphere[0].transform);


        if (hovered)
        {
            showContent();
            hovered = false;
        }

         else
        {
            Background.localScale = Vector3.Slerp(Background.localScale, minBackgroundScale, BackgroundExpansionSpeed * Time.deltaTime);
            Background.localPosition = Vector3.Slerp(Background.localPosition, new Vector3(0, -0.1f, 0), BackgroundExpansionSpeed * Time.deltaTime);
            Content.SetActive(false);
        }
    }

    public override bool OnHover(Transform t)
    {
        hovered = true;
        return false;
    }
}
