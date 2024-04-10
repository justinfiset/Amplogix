using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisconnectionTile : Tile
{
    public ElectricComponent source;
    private ElectricComponent target;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (isHover)
        {
            if (Input.GetMouseButtonDown(0))
            {
                source.GetComponent<Connection>().DeleteConnection(target);
            }
        }
    }
    public void Setup(TilesManager manager, TilePosition position, ElectricComponent source, ElectricComponent target)
    {
        base.Setup(manager, position);
        this.source = source;
        this.target = target;
    }
}
