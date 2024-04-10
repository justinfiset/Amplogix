using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Resistor;

public class Diode : ElectricComponent
{
    public float diode { get; private set; } = 0f; 

    private string diodeText = "";
    private bool isInputWrong = false;

    public override void Setup()
    {
        GUIHeightDivider = 2.5f;

        diodeText = diode.ToString();
    }

    public override void UnpackCustomComponentData(string customDataString)
    {
        ResistorData data = UnserializeCustomComponentData<ResistorData>(customDataString);
        this.diode = data.resistance;
    }

    public override string GetCustomComponentData()
    {
        return SerializeCustomComponentData(new ResistorData(diode));
    }

    public override void RenderGUI()
    {
        GUIStyle inputStyle = isInputWrong ? ComponentGUI.deleteStyle : ComponentGUI.inputStyle;
        GUIStyle labelStyle = ComponentGUI.labelStyle;

        Rect inputRect = ComponentGUI.CreateRect(0, 1, 1, 4);
        diodeText = GUI.TextField(inputRect, diodeText, 15, inputStyle);

        Rect labelRect = ComponentGUI.CreateRect(4, 1, 5, 4);
        GUI.Label(labelRect, "Ω", labelStyle);

        if (GUI.changed)
        {
            try
            {
                diode = float.Parse(diodeText);
                isInputWrong = false;
            }
            catch
            {
                isInputWrong = true;
            }
        }
    }
}
