using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;

public class PowerSource : ElectricComponent
{
    public Sprite verticalSprite;
    public Sprite horizontalSprite;

    public float voltage { get; private set; } = 5f;
    private string voltageText = "";
    private bool isInputWrong = false;

    public override void Setup()
    {
        GUIHeightDivider = 2.5f;

        voltageText = voltage.ToString();
        UpdateSprite();
    }

    public void UpdateSprite()
    {
        if (transform.localRotation.eulerAngles.z % 180 == 0) // si horizontal
        {
            sprite.sprite = horizontalSprite;
        }
        else
        {
            sprite.sprite = verticalSprite;
        }
    }

    public override void RotateComponent()
    {
        base.RotateComponent();
        UpdateSprite();
    }

    public ElectricComponent GetPositiveSideConnection()
    {
        Connection connectionComponent = GetComponent<Connection>();
        float rotation = transform.rotation.z;

        if (rotation % 1 != 0)
        {
            print("ROTATION DE LA SOURCE N'EST PAS Z?RO");
        }

        return connectionComponent.connections.connections[Connection.GetMultiplierFromConnection((int)rotation)];
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

    public override void RenderGUI()
    {
        GUIStyle inputStyle = isInputWrong ? ComponentGUI.deleteStyle : ComponentGUI.inputStyle;
        GUIStyle labelStyle = ComponentGUI.labelStyle;

        Rect inputRect = ComponentGUI.CreateRect(0, 1, 1, 4);
        voltageText = GUI.TextField(inputRect, voltageText, 10, inputStyle);

        Rect labelRect = ComponentGUI.CreateRect(4, 1, 5, 4);
        GUI.Label(labelRect, "V", labelStyle);

        if(GUI.changed)
        {
            try
            {
                voltage = float.Parse(voltageText);
                isInputWrong = false;
            } catch
            {
                isInputWrong = true;
            }
        }
    }
}

[Serializable]
public class PowerSourceData
{
    public float voltage = 5f;

    public PowerSourceData(PowerSource component)
    {
        voltage = component.voltage;
    }
}