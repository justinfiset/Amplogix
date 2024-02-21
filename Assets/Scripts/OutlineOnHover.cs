using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoverComponent : MonoBehaviour
{
    public GameObject outlineObjectPrefab;
    public GameObject scaleEditWinglet;

    private bool isHover = false;

    private GameObject outlineInstance;

    private void OnMouseEnter()
    {
        if(outlineInstance == null)
        {
            outlineInstance = Instantiate(outlineObjectPrefab);
            transform.SetParent(outlineInstance.transform);

            // copy transform
            outlineInstance.transform.position = gameObject.transform.position;
            outlineInstance.transform.localScale = gameObject.transform.localScale;
        }
    }

    private void OnMouseExit()
    {
        isHover = false;
        Destroy(outlineInstance);
    }
}
