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

    private GameObject showOnSelection;
    [SerializeField] private TextMeshPro fontSizeText;

    [HideInInspector] public TextMeshPro text;
    [HideInInspector] public RectTransform rect;
    [HideInInspector] public BoxCollider2D col;

    override public void Setup()
    {
        text = GetComponent<TextMeshPro>();
        rect = GetComponent<RectTransform>();
        col = GetComponent<BoxCollider2D>();
        showOnSelection = fontSizeText.transform.parent.gameObject;
        showOnSelection.SetActive(false);
        UpdateFontSizeText();
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
            UpdateFontSizeText();
        }
    }

    public void DecreaesTextSize()
    {
        if (text.fontSize > minTextSize)
        {
            text.fontSize -= textSizeIncrement;
            UpdateFontSizeText();
        }
    }

    public void UpdateFontSizeText()    
    {
        fontSizeText.text = text.fontSize.ToString();
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
        showOnSelection.SetActive(true);
    }

    public override void Unselect() {
        text.color = Color.black;
        listenToInputs = true;
        showOnSelection.SetActive(false);
    }

    public override void UnpackCustomComponentData(string customDataString)
    {
        TextLabelData customData = UnserializeCustomComponentData<TextLabelData>(customDataString);
        UpdateText(customData.text);
        text.fontSize = customData.fontSize;
        UpdateFontSizeText();
    }

    public override string GetCustomComponentData()
    {
        return SerializeCustomComponentData(new TextLabelData(this));
    }

    // test gui TODO FINISH GUI IMPLEMETANTAION
    private void OnGUI()
    {
        GUIStyle currentStyle = ComponentGUI.InitGUI();
        GUIStyle buttonStyle = ComponentGUI.buttonStyle;
        ComponentGUILayout layout = ComponentGUI.currentLayout;

        ComponentGUI.CreateBackground();

        // NOTE : LE RECT FONCTIONNE COMME UNE GRILLE DANS L'ESPACE DEDIÉ
        // Make the first button. If it is pressed, Application.Loadlevel (1) will be executed
        if (GUI.Button(ComponentGUI.CreateRect(0, 0, 4), "Level 1", buttonStyle))
        {
            //Application.LoadLevel(1);
        }

        // Make the second button.
        if (GUI.Button(ComponentGUI.CreateRect(1, 0, 4), "Level 2"))
        {
            //Application.LoadLevel(2);
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
