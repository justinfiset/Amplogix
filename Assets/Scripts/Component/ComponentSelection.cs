using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ComponentSelection : MonoBehaviour, IPointerClickHandler
{
    public GameObject prefab;
    private bool buttonPressed = false;
    private Image border;
    public Color selectedColor = new Color(66, 135, 245);
    public bool isDefaultSelection = false;

    private void Start()
    {
        border = transform.GetChild(0).GetComponent<Image>();

        if (isDefaultSelection) 
        { 
            SetActivePrefab();
        }
    }

    private void Update()
    {
        if (buttonPressed)
        {
            SetActivePrefab();
        }
    }

    public void SetActivePrefab()
    {
        ComponentSpawner.SetCurrentSelection(prefab, this);
    }

    public void OnSelect()
    {
        border.color = selectedColor;
    }

    public void OnUnselect()
    {
        border.color = Color.white;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        SetActivePrefab();
    }
}
