using System.Windows.Forms;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ComponentSelection : MonoBehaviour, IPointerClickHandler
{
    public ElectricComponentType type;
    public Sprite sprite;

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
        if(isDefaultSelection)
        {
            if(Input.GetKeyDown(KeyCode.Escape))
            {
                SetActivePrefab();
            }
        }
    }

    public void SetActivePrefab()
    {
        ComponentSpawner.SetCurrentSelection(type, this);
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
