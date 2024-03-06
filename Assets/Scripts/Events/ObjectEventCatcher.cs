using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ObjectEventCatcher : MonoBehaviour
{
    public UnityEvent onMouseDown;
    public UnityEvent onMouseEnter;
    public UnityEvent onMouseExit;

    private void OnMouseDown()
    {
        onMouseDown.Invoke();
    }

    private void OnMouseEnter()
    {
        onMouseEnter.Invoke();
    }

    private void OnMouseExit()
    {
        onMouseExit.Invoke();
    }
}
