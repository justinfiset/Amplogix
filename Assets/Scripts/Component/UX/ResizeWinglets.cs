using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

public class ResizeWinglets : MonoBehaviour
{
    public bool resizeVertical = true;
    public bool resizeHorizontal = true;

    public GameObject prefab;
    public Transform parent;

    public void GenerateWinglets(Vector3 origin, Vector3 scale)
    {
        parent = new GameObject("Resize Wignlets").transform;

        if(resizeVertical) // top / bottom
        {
            Vector3 topPos = new Vector3(origin.x, origin.y + (scale.y / 2), 0);
            Vector3 bottomPos = new Vector3(origin.x, origin.y - (scale.y / 2), 0);
            Quaternion angles = Quaternion.Euler(0f, 0f, 90f);
            Instantiate(prefab, topPos, angles, parent);
            Instantiate(prefab, bottomPos, angles, parent);
        }
        if(resizeHorizontal) // left / right
        {
            Vector3 leftPos = new Vector3(origin.x - (scale.x / 2), origin.y, 0);
            Vector3 rightPos = new Vector3(origin.x + (scale.x / 2), origin.y, 0);
            Quaternion angles = Quaternion.identity;
            Instantiate(prefab, leftPos, angles, parent);
            Instantiate(prefab, rightPos, angles, parent);
        }
    }

    public void DestroyWinglets()
    {
        Destroy(parent);
    }
}
