using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Resistor;

public class Moteur : ElectricComponent
{
    public float moteur { get; private set; } = 0f;

    private string moteurText = "";
    private bool isInputWrong = false;

    public override void Setup()
    {
        GUIHeightDivider = 2.5f;

        moteurText = moteur.ToString();
    }

    public override void UnpackCustomComponentData(string customDataString)
    {
        ResistorData data = UnserializeCustomComponentData<ResistorData>(customDataString);
        this.moteur = data.resistance;
    }

    public override string GetCustomComponentData()
    {
        return SerializeCustomComponentData(new ResistorData(moteur));
    }

    public override void RenderGUI()
    {
        GUIStyle inputStyle = isInputWrong ? ComponentGUI.deleteStyle : ComponentGUI.inputStyle;
        GUIStyle labelStyle = ComponentGUI.labelStyle;

        Rect inputRect = ComponentGUI.CreateRect(0, 1, 1, 4);
        moteurText = GUI.TextField(inputRect, moteurText, 15, inputStyle);

        Rect labelRect = ComponentGUI.CreateRect(4, 1, 5, 4);
        GUI.Label(labelRect, "Ω", labelStyle);

        if (GUI.changed)
        {
            try
            {
                moteur = float.Parse(moteurText);
                isInputWrong = false;
            }
            catch
            {
                isInputWrong = true;
            }
        }
    }
}
