using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Resistor;

public class Ampoule : ElectricComponent
{
    public float ampoule { get; private set; } = 0f;

    private string ampouleText = "";
    private bool isInputWrong = false;

    public override void Setup()
    {
        GUIHeightDivider = 2.5f;

        ampouleText = ampoule.ToString();
    }

    public override void SetCalculatedIntensity(float calculatedIntensity)
    {
        base.SetCalculatedIntensity(calculatedIntensity);

        if (isLightSource)
        {
            GetComponent<LightSource>().SetIntensity(currentIntensity);
        }
    }

    public override void UnpackCustomComponentData(string customDataString)
    {
        ResistorData data = UnserializeCustomComponentData<ResistorData>(customDataString);
        this.ampoule = data.resistance;
    }

    public override string GetCustomComponentData()
    {
        return SerializeCustomComponentData(new ResistorData(ampoule));
    }

    public override void RenderGUI()
    {
        GUIStyle inputStyle = isInputWrong ? ComponentGUI.deleteStyle : ComponentGUI.inputStyle;
        GUIStyle labelStyle = ComponentGUI.labelStyle;

        Rect inputRect = ComponentGUI.CreateRect(0, 1, 1, 4);
        ampouleText = GUI.TextField(inputRect, ampouleText, 15, inputStyle);

        Rect labelRect = ComponentGUI.CreateRect(4, 1, 5, 4);
        GUI.Label(labelRect, "Ω", labelStyle);

        if (GUI.changed)
        {
            try
            {
                ampoule = float.Parse(ampouleText);
                isInputWrong = false;
            }
            catch
            {
                isInputWrong = true;
            }
        }
    }
}
