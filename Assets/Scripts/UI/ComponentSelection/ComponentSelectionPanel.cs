using System.Collections;
using System.Collections.Generic;
using System.Windows.Forms;
using UnityEngine;

public class ComponentSelectionPanel : MonoBehaviour
{
    public GameObject panel;
    public GameObject arrow;

    private float originalPosition;
    public float positionOffset = 20f;
    public float animationDuration = 0.2f;
    private bool isPanelHidden = false;

    public void Start()
    {
        originalPosition = panel.transform.localPosition.x;
    }

    public void Show()
    {
        LeanTween.moveLocalX(panel, originalPosition, animationDuration);
        LeanTween.rotateY(arrow, 0f, animationDuration);
    }

    public void Hide()
    {
        LeanTween.moveLocalX(panel, (originalPosition - positionOffset), animationDuration);
        LeanTween.rotateY(arrow, 180f, animationDuration);
    }

    public void ToggglePanel()
    {
        if (isPanelHidden) Show();
        else Hide();

        isPanelHidden = !isPanelHidden;
    }
}
