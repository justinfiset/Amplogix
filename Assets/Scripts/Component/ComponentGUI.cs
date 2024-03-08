using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComponentGUI : MonoBehaviour
{
    public static GUIStyle currentStyle = null;
    public static GUIStyle buttonStyle = null;
    public static ComponentGUILayout currentLayout;

    public static GUIStyle InitGUI()
    {
        InitLayout();
        return InitStyles();
    }

    public static ComponentGUILayout InitLayout()
    {
        if(currentLayout == null)
        {
            currentLayout = new ComponentGUILayout();
        }

        return currentLayout;
    }

    public static GUIStyle InitStyles()
    {
        if (currentStyle == null)
        {
            currentStyle = new GUIStyle(GUI.skin.box);
            currentStyle.normal.background = MakeTex(2, 2, Color.white);
            currentStyle.normal.textColor = Color.black;
            currentStyle.onActive.background = MakeTex(2, 2, Color.gray);
        }

        if(buttonStyle == null)
        {
            buttonStyle = new GUIStyle(GUI.skin.button);
        }

        return currentStyle;
    }

    public static Texture2D MakeTex(int width, int height, Color col)
    {
        Color[] pix = new Color[width * height];
        for (int i = 0; i < pix.Length; ++i)
        {
            pix[i] = col;
        }
        Texture2D result = new Texture2D(width, height);
        result.SetPixels(pix);
        result.Apply();
        return result;
    }

    public static void CreateBackground()
    {
        GUI.Box(new Rect(currentLayout.x, currentLayout.y, currentLayout.width, currentLayout.height), "Text Label", currentStyle);
    }

    public static Rect CreateRect(float col, float row, float widthDiv, float heightDiv = 20)
    {
        float width = currentLayout.width / widthDiv;
        float height = currentLayout.height / heightDiv;
        float x = currentLayout.x + currentLayout.padding / 2 * (col + 1) + col / widthDiv * currentLayout.width;
        float y = currentLayout.y + currentLayout.padding * (row + 1) + row / height * currentLayout.height;
        return new Rect(x, y, width, height);
    }
}

public class ComponentGUILayout
{
    public float padding = 20f;
    public float height;
    public float width;
    public float x;
    public float y;

    public ComponentGUILayout()
    {
        height = Screen.height / 2;
        width = Screen.width / 4;
        x = Screen.width - width - padding;
        y = Screen.height - height - padding;
    }
}
