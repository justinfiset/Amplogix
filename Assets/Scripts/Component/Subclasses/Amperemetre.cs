using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Amperemetre : ElectricComponent
{
    private string currentText = "";

    public override void Setup()
    {
        GUIHeightDivider = 2.5f;

        currentText = resistance.ToString();
    }

    public override void OnCurrentChange(float newCurrent)
    {
        currentText = Math.Round(currentIntensity, 2).ToString();
    }

    public override void RenderGUI()
    {
        GUIStyle inputStyle = ComponentGUI.inputStyle;
        GUIStyle labelStyle = ComponentGUI.labelStyle;

        Rect inputRect = ComponentGUI.CreateRect(0, 1, 1, 4);
        GUI.TextField(inputRect, currentText, 15, inputStyle);

        Rect labelRect = ComponentGUI.CreateRect(4, 1, 5, 4);
        GUI.Label(labelRect, "A", labelStyle);
    }
}
    
