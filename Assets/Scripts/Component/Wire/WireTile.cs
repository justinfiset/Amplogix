using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum WireTilePosition
{
    Top = 270,
    Bottom = 90,
    Left = 180,
    Right = 0
}

public class WireTile : MonoBehaviour
{
    private bool isHover = false;
    private WireTilesManager manager;
    public WireTilePosition position;

    public void Update()
    {
        if(isHover)
        {
            if(Input.GetMouseButtonDown(0))
            {
                manager.CreateNewWire(this);
            }
        }
    }

    public void Setup(WireTilesManager manager, WireTilePosition position)
    {
        this.manager = manager;
        this.position = position;
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
