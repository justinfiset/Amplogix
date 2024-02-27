using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WireTile : MonoBehaviour
{
    private bool isHover = false;

    public void Update()
    {
        if(isHover)
        {
            if(Input.GetMouseButtonDown(0))
            {
                ElectricComponentType type = ElectricComponentType.Wire;
                Quaternion angles = Quaternion.identity;
                // TODO GERER ORIENTATION
                ComponentSpawner.CreateComponent(type, transform.position, angles, Vector3.one);
            }
        }
    }

    #region Mouse callbacks
    private void OnMouseEnter()
    {
        isHover = true;
    }

    private void OnMouseExit()
    {
        isHover = false;
    }
    #endregion
}
