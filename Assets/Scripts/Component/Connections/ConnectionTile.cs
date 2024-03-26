using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ConnectionTile : Tile
{
    public ElectricComponent source;
    public bool isHorizontal;

    public void Update()
    {
        if (isHover)
        {
            if (Input.GetMouseButtonDown(0))
            {
                ElectricComponent second;
                second = ProjectManager.m_Instance.GetComponent(gameObject.transform.localPosition);
                ProjectManager.m_Instance.ConnectComponents(source, second);
            }
        }
    }

    public void Setup(TilesManager manager, TilePosition position, bool isHorizontal, ElectricComponent source)
    {
        base.Setup(manager, position);
        this.isHorizontal = isHorizontal;
        this.source = source;
    }

}
