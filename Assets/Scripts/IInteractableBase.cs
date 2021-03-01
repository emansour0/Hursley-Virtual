using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInteractableBase
{
    public void OnClick();

    public bool OnHover(Transform transform);
}
