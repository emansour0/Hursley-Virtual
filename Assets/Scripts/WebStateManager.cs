using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class WebStateManager : MonoBehaviour
{
    public static UnityEvent OnApplicationFocusEvent = new UnityEvent();
    public static UnityEvent OnApplicationUnfocusEvent = new UnityEvent();

    void OnApplicationFocus(bool hasFocus)
    {
        if(hasFocus)
        {
            OnApplicationFocusEvent.Invoke();
        }
        else
        {
            OnApplicationUnfocusEvent.Invoke();
        }
    }
}
