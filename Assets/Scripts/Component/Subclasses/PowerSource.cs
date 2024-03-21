using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;

public class PowerSource : ElectricComponent
{
    public Sprite verticalSprite;
    public Sprite horizontalSprite;

    public float voltage = 0;
    private string voltageString = "";
    private bool isInputWrong = false;

    public override void Setup()
    {
        GUIHeightDivider = 2.5f;
    }

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

    string voltageText = "";

    public override void RenderGUI()
    {
        GUIStyle inputStyle = isInputWrong ? ComponentGUI.deleteStyle : ComponentGUI.inputStyle;

        Rect inputRect = ComponentGUI.CreateRect(0, 1, 1, 4);
        voltageText = GUI.TextField(inputRect, voltageText, 10, inputStyle);
        if(GUI.changed)
        {
            try
            {
                voltage = float.Parse(voltageText);
                isInputWrong = false;
            } catch (Exception e)
            {
                isInputWrong = true;
            }
        }
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