using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Resistor;

public class Condensateur  : ElectricComponent
{
    public float condensateur { get; private set; } = 10f;

    private string condensateurText = "";
    private bool isInputWrong = false;

    public override void Setup()
    {
        GUIHeightDivider = 2.5f;

        condensateurText = condensateur.ToString();
    }

    public override void UnpackCustomComponentData(string customDataString)
    {
        ResistorData data = UnserializeCustomComponentData<ResistorData>(customDataString);
        this.condensateur = data.resistance;
    }

    public override string GetCustomComponentData()
    {
        return SerializeCustomComponentData(new ResistorData(condensateur));
    }

    public override void RenderGUI()
    {
        GUIStyle inputStyle = isInputWrong ? ComponentGUI.deleteStyle : ComponentGUI.inputStyle;
        GUIStyle labelStyle = ComponentGUI.labelStyle;

        Rect inputRect = ComponentGUI.CreateRect(0, 1, 1, 4);
        condensateurText = GUI.TextField(inputRect, condensateurText, 15, inputStyle);

        Rect labelRect = ComponentGUI.CreateRect(4, 1, 5, 4);
        GUI.Label(labelRect, "C", labelStyle);

        if (GUI.changed)
        {
            try
            {
                condensateur = float.Parse(condensateurText);
                isInputWrong = false;
            }
            catch
            {
                isInputWrong = true;
            }
        }
    }
}
