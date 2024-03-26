using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WireTile : Tile
{
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
}
