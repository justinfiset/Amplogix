using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TextLabel : ElectricComponent
{
    [HideInInspector] public TextMeshPro text;
    [HideInInspector] public RectTransform rect;

    override public void Setup()
    {
        text = GetComponent<TextMeshPro>();
        rect = GetComponent<RectTransform>();
    }

    public void UpdateName(string newName)
    {
        text.text = newName;
        outline.transform.localScale = rect.sizeDelta;
    }

    public override void RotateComponent() { }

    public override void Select() {
        outline.transform.localScale = rect.sizeDelta;
        text.color = text.color * new Color(1, 1, 1, 0.25f);
    }

    public override void Unselect() {
        text.color = Color.black;
    }

    public override void UnpackCustomComponentData(string customDataString)
    {
        TextLabelData customData = UnserializeCustomComponentData<TextLabelData>(customDataString);
        UpdateName(customData.text);
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
