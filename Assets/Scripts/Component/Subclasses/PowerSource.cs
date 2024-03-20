using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerSource : ElectricComponent
{
    public Sprite verticalSprite;
    public Sprite horizontalSprite;

    public float voltage = 0;

    public override void RotateComponent()
    {
        base.RotateComponent();
        if(transform.localRotation.eulerAngles.z % 180 == 0) // si horizontal
        {
            sprite.sprite = horizontalSprite;
        }
        else
        {
            sprite.sprite = verticalSprite;
        }
    }

    public override void UnpackCustomComponentData(string customDataString)
    {
        PowerSourceData data = UnserializeCustomComponentData<PowerSourceData>(customDataString);
        this.voltage = data.voltage;
    }

    public override string GetCustomComponentData()
    {
        return SerializeCustomComponentData(new PowerSourceData(this));
    }
}

[Serializable]
public class PowerSourceData
{
    public float voltage = 9;

    public PowerSourceData(PowerSource component)
    {
        voltage = component.voltage;
    }
}