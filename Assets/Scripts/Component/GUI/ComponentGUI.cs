using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ComponentGUI
{
    public static Color[] colorList = null;
    public static GUIStyle[] colorStyle = null;
    public static int colorRowCount = 2;
    public static int colorColCount = 5;

    public static GUIStyle currentStyle = null;
    public static GUIStyle buttonStyle = null;
    public static GUIStyle deleteStyle = null;
    public static GUIStyle labelStyle = null;
    public static GUIStyle inputStyle = null;
    public static ComponentGUILayout currentLayout;

    public static float padding = 20f;

    public static bool InitGUI()
    {
        int fontSize = 32;
        GUI.skin.box.fontSize = fontSize;
        GUI.skin.button.fontSize = fontSize;
        return InitStyles();
    }

    public static ComponentGUILayout InitLayout(float heightDivider = 3f)
    {
        if(currentLayout == null || heightDivider != currentLayout.yDividier)
        {
            currentLayout = new ComponentGUILayout(heightDivider);
        }
        return currentLayout;
    }

    public static bool InitStyles()
    {
        //if(currentStyle == null)
        //{
            currentStyle = new GUIStyle(GUI.skin.box);
            currentStyle.normal.background = MakeTex(2, 2, Color.white);
            currentStyle.normal.textColor = Color.black;
        //}

        //if(buttonStyle == null)
        //{
            buttonStyle = new GUIStyle(GUI.skin.box);
            buttonStyle.alignment = TextAnchor.MiddleCenter;
            buttonStyle.normal.background = MakeTex(2, 2, new Color32(49, 106, 189, 255));
            buttonStyle.normal.textColor = Color.white;
            buttonStyle.hover.background = MakeTex(2, 2, new Color32(20, 56, 105, 255));
            buttonStyle.hover.textColor = Color.white;
            buttonStyle.active.background = MakeTex(2, 2, new Color32(19, 34, 56, 255));
            buttonStyle.active.textColor = Color.white;
        //}

        //if(deleteStyle == null)
        //{
            deleteStyle = new GUIStyle(GUI.skin.box);
            deleteStyle.alignment = TextAnchor.MiddleCenter;
            deleteStyle.normal.background = MakeTex(2, 2, new Color32(191, 54, 54, 255));
            deleteStyle.normal.textColor = Color.white;
            deleteStyle.hover.background = MakeTex(2, 2, new Color32(125, 30, 30, 255));
            deleteStyle.hover.textColor = Color.white;
            deleteStyle.active.background = MakeTex(2, 2, new Color32(59, 12, 9, 255));
            deleteStyle.active.textColor = Color.white;
        //}

        //if (labelStyle == null)
        //{
            labelStyle = new GUIStyle(currentStyle);
            labelStyle.alignment = TextAnchor.MiddleCenter;
        //}

        //if (inputStyle == null)
        //{
            inputStyle = new GUIStyle(buttonStyle);
            inputStyle.alignment = TextAnchor.MiddleLeft;
            inputStyle.padding.left = (int)padding;
            inputStyle.normal.background = MakeTex(2, 2, new Color32(52, 104, 179, 255));
        //}

        GenerateColorList();
        return true;
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

    public static Rect CreateBackground(ElectricComponent component, string headerName)
    {
        if (currentStyle == null) return new Rect(0,0,0,0);
        Rect rect = new Rect(currentLayout.x, currentLayout.y, currentLayout.width, currentLayout.height);
        GUI.Box(rect, headerName, currentStyle);
        component.isMouseOverGUI = rect.Contains(Event.current.mousePosition);
        return rect;
    }

    public static void CreateDeleteButton()
    {
        float height = 40;
        float x = Screen.width - currentLayout.width;
        float y = Screen.height - height - currentLayout.padding * 2;
        if (GUI.Button(new Rect(x, y, currentLayout.width - 2 * currentLayout.padding, height), "Supprimer", deleteStyle))
        {
            ProjectManager.DeleteSelectedComponents();
        }
    }

    public static Rect CreateRect(float col, float row, float widthDiv, float heightDiv = 20)
    {
        float width = (currentLayout.width - currentLayout.padding * (widthDiv + 1)) / widthDiv;
        float height = currentLayout.height / heightDiv;
        float x = currentLayout.x + (col + 1) * currentLayout.padding + (col) * width;
        float y = currentLayout.y + currentLayout.padding * (row + 1) + row / height * currentLayout.height;
        return new Rect(x, y, width, height);
    }

    private static void GenerateColorList()
    {
        List<Color> list = new List<Color>();
        list.Add(new Color32(255, 105, 97, 255)); // rouge 
        list.Add(new Color32(255, 180, 128, 255)); // oragne 
        list.Add(new Color32(248, 243, 141, 255)); // jaune
        list.Add(new Color32(66, 214, 164, 255)); // vert
        list.Add(new Color32(8, 202, 209, 255)); // turquoise
        list.Add(new Color32(89, 173, 246, 255)); // bleu
        list.Add(new Color32(157, 148, 255, 255)); // mauve
        list.Add(new Color32(199, 128, 232, 255)); // rose
        list.Add(new Color32(255, 255, 255, 255)); // blanc
        list.Add(new Color32(0, 0, 0, 255)); // noir
        colorList = list.ToArray();

        colorStyle = new GUIStyle[10];
        for (int i = 0; i < colorList.Length; i++)
        {
            GUIStyle style = new GUIStyle(GUI.skin.box);
            style.normal.background = MakeTex(2, 2, colorList[i]);
            colorStyle[i] = style;
        }
    }

    public static void CreateColorPalette()
    {
        float size = (currentLayout.width - 2 * currentLayout.padding) / colorColCount;

        // 2 row of 5 cokumns
        int count = 0;
        for(int i = 0; i < colorRowCount; i++)
        {
            float y = currentLayout.y + currentLayout.height - 40 - 2 * currentLayout.padding - 2 * size + (i * size);
            for (int j = 0; j < colorColCount; j++)
            {
                float x = currentLayout.x + j * size + currentLayout.padding;
                Rect rect = new Rect(x, y, size, size);

                if (GUI.Button(rect, "", colorStyle[count]))
                {
                    ProjectManager.ChangeSelectionColor(colorList[count]);
                }
                count++;
            }
        }
    }
}

public class ComponentGUILayout
{
    public float yDividier;
    public float padding = 20f;
    public float height;
    public float width;
    public float x;
    public float y;

    public ComponentGUILayout(float heightDivider)
    {
        yDividier = heightDivider;
        height = Screen.height / heightDivider;
        width = Screen.width / 4;
        x = Screen.width - width - padding;
        y = Screen.height - height - padding;
    }
}
