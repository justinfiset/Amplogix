﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Resistor;

public class Voltmetre : ElectricComponent
{
    public float voltmetre { get; private set; } = 0f; //Modifier la valeur lorsque l'accessibilité des données sera plus simple

    private string voltmetreText = "";
    private bool isInputWrong = false;

    public override void Setup()
    {
        GUIHeightDivider = 2.5f;

        voltmetreText = voltmetre.ToString();
    }

    public override void UnpackCustomComponentData(string customDataString)
    {
        ResistorData data = UnserializeCustomComponentData<ResistorData>(customDataString);
        this.voltmetre = data.resistance;
    }

    public override string GetCustomComponentData()
    {
        return SerializeCustomComponentData(new ResistorData(voltmetre));
    }

    public override void RenderGUI()
    {
        GUIStyle inputStyle = isInputWrong ? ComponentGUI.deleteStyle : ComponentGUI.inputStyle;
        GUIStyle labelStyle = ComponentGUI.labelStyle;

        Rect inputRect = ComponentGUI.CreateRect(0, 1, 1, 4);
        voltmetreText = GUI.TextField(inputRect, voltmetreText, 15, inputStyle);

        Rect labelRect = ComponentGUI.CreateRect(4, 1, 5, 4);
        GUI.Label(labelRect, "V", labelStyle);

        if (GUI.changed)
        {
            try
            {
                voltmetre = float.Parse(voltmetreText);
                isInputWrong = false;
            }
            catch
            {
                isInputWrong = true;
            }
        }
    }
}
