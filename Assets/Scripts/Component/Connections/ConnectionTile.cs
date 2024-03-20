using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ConnectionTilePosition
{
    Top = 270,
    Bottom = 90,
    Left = 180,
    Right = 0
}

public class ConnectionTile : MonoBehaviour
{
    private bool isHover = false;
    private TilesManager manager;
    public ConnectionTilePosition position;
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
                ProjectManager.m_Instance.ConnectComponents(source, second); //todo: changer pour connecter
            }
        }
    }

    public void Setup(TilesManager manager, ConnectionTilePosition position, bool isHorizontal, ElectricComponent source)
    {
        this.manager = manager;
        this.position = position;
        this.isHorizontal = isHorizontal;
        this.source = source;
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
