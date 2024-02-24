using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerSource : ElectricComponent
{
    public Sprite verticalSprite;
    public Sprite horizontalSprite;

    public override void RotateComponent()
    {
        base.RotateComponent();
        // todo changer le sprite au bon
    }
}
