using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NonExpandingPopupController : PopupController
{
    public float HeightOffset;

    protected override void Start()
    {
        PlayerHeightOffset = HeightOffset;
    }

    // Update is called once per frame
    protected override void Update()
    {
        int layerMask = 1 << PlayerLayer;

        Collider[] visibiltySphere = Physics.OverlapSphere(transform.position, FollowPlayerRadius, layerMask);
        Collider[] contentSphere = Physics.OverlapSphere(transform.position, ShowContentRadius, layerMask);

        if (visibiltySphere.Length > 0) rotateToPlayer(visibiltySphere[0].transform);
    }
}
