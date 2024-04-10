using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Resistor;

public class Amperemetre : ElectricComponent
{
    public float amperemetre { get; private set; } = 0f; //Modifier la valeur lorsque l'accessibilité des données sera plus simple

    private string amperemetreText = "";
    private bool isInputWrong = false;

    public override void Setup()
    {
        GUIHeightDivider = 2.5f;

        amperemetreText = amperemetre.ToString();
    }

    public override void UnpackCustomComponentData(string customDataString)
    {
        ResistorData data = UnserializeCustomComponentData<ResistorData>(customDataString);
        this.amperemetre = data.resistance;
    }

    public override string GetCustomComponentData()
    {
        return SerializeCustomComponentData(new ResistorData(amperemetre));
    }

    public override void RenderGUI()
    {
        GUIStyle inputStyle = isInputWrong ? ComponentGUI.deleteStyle : ComponentGUI.inputStyle;
        GUIStyle labelStyle = ComponentGUI.labelStyle;

        Rect inputRect = ComponentGUI.CreateRect(0, 1, 1, 4);
        amperemetreText = GUI.TextField(inputRect, amperemetreText, 15, inputStyle);

        Rect labelRect = ComponentGUI.CreateRect(4, 1, 5, 4);
        GUI.Label(labelRect, "A", labelStyle);

        if (GUI.changed)
        {
            try
            {
                amperemetre = float.Parse(amperemetreText);
                isInputWrong = false;
            }
            catch
            {
                isInputWrong = true;
            }
        }
    }
}
    
