﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Voltmetre : ElectricComponent
{
    private string potentialText = "";

    public override void Setup()
    {
        GUIHeightDivider = 2.5f;

        potentialText = componentPotential.ToString();
    }

    public override void RenderGUI()
    {
        GUIStyle inputStyle = ComponentGUI.inputStyle;
        GUIStyle labelStyle = ComponentGUI.labelStyle;

        Rect inputRect = ComponentGUI.CreateRect(0, 1, 1, 4);
        potentialText = componentPotential.ToString();
        potentialText = GUI.TextField(inputRect, potentialText, 15, inputStyle);

        Rect labelRect = ComponentGUI.CreateRect(4, 1, 5, 4);
        GUI.Label(labelRect, "V", labelStyle);
    }
}
