using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TextLabel : ElectricComponent
{
    private Vector2 m_Offset = new Vector2(0.3f, 0);
    [HideInInspector] public TextMeshPro text;
    [HideInInspector] public RectTransform rect;
    [HideInInspector] public BoxCollider2D collider;

    override public void Setup()
    {
        text = GetComponent<TextMeshPro>();
        rect = GetComponent<RectTransform>();
        collider = GetComponent<BoxCollider2D>();
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

    public void UpdateText(string newText)
    {
        text.text = newText;
        UpdateSize();
    }

    public override void RotateComponent() { }

    private void UpdateSize()
    {
        outline.transform.localScale = rect.sizeDelta + m_Offset;
        collider.size = rect.sizeDelta + m_Offset;
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
    }

    public override string GetCustomComponentData()
    {
        return SerializeCustomComponentData(new TextLabelData(this));
    }
}

[Serializable]
public class TextLabelData
{
    public string text;

    public TextLabelData(TextLabel component) {
        text = component.text.text;
    }
}
