using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DisableCameraOnHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public void OnPointerEnter(PointerEventData eventData)
    {
        CameraBehaviour.DisableControllers();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        CameraBehaviour.EnableControllers();
    }
}
