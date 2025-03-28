﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Resistor;

public class Diode : ElectricComponent
{
    private string resistanceText = "";
    private bool isInputWrong = false;

    public override void Setup()
    {
        GUIHeightDivider = 2.5f;

        SetBaseResistance(0.1f);

        resistanceText = resistance.ToString();
    }

    public override void RenderGUI()
    {
        GUIStyle inputStyle = isInputWrong ? ComponentGUI.deleteStyle : ComponentGUI.inputStyle;
        GUIStyle labelStyle = ComponentGUI.labelStyle;

        Rect inputRect = ComponentGUI.CreateRect(0, 1, 1, 4);
        resistanceText = GUI.TextField(inputRect, resistanceText, 15, inputStyle);

        Rect labelRect = ComponentGUI.CreateRect(4, 1, 5, 4);
        GUI.Label(labelRect, "Ω", labelStyle);

        if (GUI.changed)
        {
            try
            {
                resistance = float.Parse(resistanceText);
                isInputWrong = false;
            }
            catch
            {
                isInputWrong = true;
            }
        }
    }
}
