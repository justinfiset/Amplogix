using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TextLabel : ElectricComponent
{
    private float m_Offset = 0.3f;

    private float textSizeIncrement = 1;
    private float minTextSize = 1;
    private float maxTextSize = 14;

    [HideInInspector] public TextMeshPro text;
    [HideInInspector] public RectTransform rect;
    [HideInInspector] public BoxCollider2D col;

    override public void Setup()
    {
        text = GetComponent<TextMeshPro>();
        rect = GetComponent<RectTransform>();
        col = GetComponent<BoxCollider2D>();
    }

    override public void OnUpdate()
    {
        if(isSelected)
        {
            foreach (char c in Input.inputString)
            {
                if (c == '\b')
                {
                    if (text.text.Length != 0)
                    {
                        text.text = text.text.Substring(0, text.text.Length - 1);
                        UpdateSize();
                    }
                }
                else if ((c == '\n') || (c == '\r'))
                {
                    UpdateSize();
                    _Unselect();
                }
                else
                {
                    UpdateText(text.text += c);
                }
            }
        }
    }

    public void IncreaseTextSize()
    {
        if(text.fontSize < maxTextSize)
        {
            text.fontSize += textSizeIncrement;
        }
    }

    public void DecreaesTextSize()
    {
        if (text.fontSize > minTextSize)
        {
            text.fontSize -= textSizeIncrement;
        }
    }

    public void UpdateText(string newText)
    {
        text.text = newText;
        UpdateSize();
    }

    public override void RotateComponent() { }

    private void UpdateSize()
    {
        col.size = new Vector2(rect.sizeDelta.x + m_Offset, col.size.y);
        outline.transform.localScale = new Vector2(rect.sizeDelta.x + m_Offset, col.size.y);
    }

    public override void Select() {
        UpdateSize();
        text.color = text.color * new Color(1, 1, 1, 0.5f);
        listenToInputs = false;
    }

    public override void Unselect() {
        text.color = Color.black;
        listenToInputs = true;
    }

    public override void UnpackCustomComponentData(string customDataString)
    {
        TextLabelData customData = UnserializeCustomComponentData<TextLabelData>(customDataString);
        UpdateText(customData.text);
        text.fontSize = customData.fontSize;
    }

    public override string GetCustomComponentData()
    {
        return SerializeCustomComponentData(new TextLabelData(this));
    }

    private void OnGUI()
    {
        if(isSelected)
        {
            ComponentGUI.InitGUI();
            GUIStyle buttonStyle = ComponentGUI.buttonStyle;
            GUIStyle labelStyle = ComponentGUI.labelStyle;
            
            // Creation de la fenetre en bas à droite
            ComponentGUI.CreateBackground(this, "Texte");

            if (GUI.Button(ComponentGUI.CreateRect(0, 1, 3, 8), "-", buttonStyle))
            {
                DecreaesTextSize();
            }

            GUI.Label(ComponentGUI.CreateRect(1, 1, 3, 8), text.fontSize.ToString(), labelStyle);

            // Make the second button.
            if (GUI.Button(ComponentGUI.CreateRect(2, 1, 3, 8), "+", buttonStyle))
            {
                IncreaseTextSize();
            }

            ComponentGUI.CreateDeleteButton(this);
        }
    }
}

[Serializable]
public class TextLabelData
{
    public string text = "";
    public float fontSize = 8;

    public TextLabelData(TextLabel component) {
        text = component.text.text;
        fontSize = component.text.fontSize;
    }
}