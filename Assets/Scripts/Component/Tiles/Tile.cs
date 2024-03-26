using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TilePosition
{
    Top = 270,
    Bottom = 90,
    Left = 180,
    Right = 0
}

public class Tile : MonoBehaviour
{
    protected bool isHover = false;
    protected TilesManager manager;
    public TilePosition position;

    public static Connection.Position GetPositionFromTilePosition(TilePosition tilePosition)
    {
        return tilePosition switch
        {
            TilePosition.Top => Connection.Position.Top,
            TilePosition.Bottom => Connection.Position.Bottom,
            TilePosition.Left => Connection.Position.Left,
            TilePosition.Right => Connection.Position.Right,
            _ => throw new System.Exception(),
        };
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public virtual void Setup(TilesManager manager, TilePosition position)
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
