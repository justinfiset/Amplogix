using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(ResizeWinglets))]
[RequireComponent(typeof(WireTilesManager))]
[RequireComponent(typeof(BoxCollider2D))]
public class ElectricComponent : MonoBehaviour
{
    [Header("Logic")]
    public ElectricComponentType type;
    public bool canBeRotated = true; // Le composant peut-il être rotationé par l'utilisateur?
    public bool canBeMoved = true; // Le composant peut-il être manipulé par l'utilisateur?
    [Header("State")]
    private bool isHover = false;
    private bool isSelected = false;
    private ElectricComponentData data;

    [Header("Move / Drag")]
    public bool hasReleasedSinceSelection = true;
    public bool isBeingMoved = false;
    public Vector3 lastClickPos;

    [Header("UI")]
    [SerializeField] private SpriteRenderer outline;
    private ResizeWinglets resizeWinglets;
    private WireTilesManager wireTilesManager;
    private SpriteRenderer sprite;

    [Header("Inputs")]
    private static KeyCode rotateKey = KeyCode.R;
    private static KeyCode mouseDeleteKey = KeyCode.Mouse2;
    private static KeyCode keyboardDeleteKey = KeyCode.Backspace;
    private static KeyCode systemDeleteKey = KeyCode.Delete;
    private static KeyCode unSelectKey = KeyCode.Escape;

    public virtual void _RotateComponent()
    {
        Unselect();
        RotateComponent();
        Select();
        ProjectManager.m_Instance.isProjectSaved = false;
    }

    public virtual void RotateComponent()
    {
        transform.Rotate(Vector3.forward * -90);
    }

    public virtual void DestroyComponent()
    {
        Unselect();
        ComponentSpawner.DestroyComponent(gameObject);
    }

    void Start()
    {
        resizeWinglets = GetComponent<ResizeWinglets>();
        wireTilesManager = GetComponent<WireTilesManager>();
        sprite = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            if (isHover)
            {
                lastClickPos = GridSettings.GetCurrentSnapedPosition();

                if (!isSelected)
                {
                    Select();
                    hasReleasedSinceSelection = false;
                }
            }
        }
        else if(Input.GetMouseButtonUp(0))
        {
            if (isSelected && hasReleasedSinceSelection)
            {
                Unselect();
            } else
            {
                hasReleasedSinceSelection = true;
            }
        }

        if (isSelected)
        {
            if (canBeRotated && Input.GetKeyDown(rotateKey))
            {
                _RotateComponent();
            }
            else if (Input.GetKeyDown(mouseDeleteKey)
                || Input.GetKeyDown(keyboardDeleteKey)
                || Input.GetKeyDown(systemDeleteKey))
            {
                DestroyComponent();
            }
            else if (Input.GetKeyDown(unSelectKey))
            {
                Unselect();
            }

            if (!hasReleasedSinceSelection)
            {
                if (!isBeingMoved)
                {
                    isBeingMoved = true;
                }

                if (isBeingMoved && canBeMoved)
                {
                    MoveComponent(GridSettings.GetCurrentSnapedPosition());
                }
            }
            else // Si on relache le boutton
            {
                if (isBeingMoved)
                {
                    print(ProjectManager.m_Instance.GetComponentCount(transform.position));
                    if (ProjectManager.m_Instance.GetComponentCount(transform.position) != 1)
                    {
                        MoveComponent(lastClickPos);
                    }
                }
            }
        }
    }

    public void MoveComponent(Vector3 newPos)
    {
        Unselect();
        transform.position = newPos;
        ProjectManager.m_Instance.ChangeComponentPos(this, transform.position);
        ProjectManager.m_Instance.isProjectSaved = false;
        Select();
    }

    public void Select()
    {
        isSelected = true;
        resizeWinglets.GenerateWinglets(transform.position, transform.localScale);
        wireTilesManager.ShowTiles();
        sprite.color = sprite.color * new Color(1, 1, 1, 0.5f);
        outline.color = Color.white;
    }

    public void Unselect()
    {
        isSelected = false;
        resizeWinglets.DestroyWinglets();
        wireTilesManager.HideTiles();
        sprite.color = Color.white;
        outline.color = Color.clear;
    }

    public void UpdateData()
    {
        if(data == null) data = new ElectricComponentData();

        data.type = (int) type;
        data.x = transform.position.x;
        data.y = transform.position.y;
        data.rot = transform.localEulerAngles.z;
        data.scaleX = transform.localScale.x;
        data.scaleY = transform.localScale.y;
    }

    public ElectricComponentData GetData()
    {
        UpdateData();
        return data;
    }

    #region Mouse Callback
    private void OnMouseEnter()
    {
        isHover = true;
    }

    private void OnMouseExit()
    {
        isHover = false;
    }
    #endregion
}

[Serializable]
public enum ElectricComponentType
{
    None,
    Resistor,
    PowerSource,
    Switch,
    LightBulb,
    Motor,
    Condensator,
    Ammeter,
    Voltmeter,
    Diode,
    Coil,
    Wire,
    WireCorner,
    WireThreeWay,
    WireFourWay
}

public static class ElectricComponentTypeMethods
{
    public static bool IsWire(ElectricComponent component)
    {
        ElectricComponentType[] wireTypes =
        {
            ElectricComponentType.Wire,
            ElectricComponentType.WireCorner,
            ElectricComponentType.WireThreeWay,
            ElectricComponentType.WireFourWay
        };

        return wireTypes.Contains(component.type);
    }
}

[Serializable]
public class ElectricComponentData
{
    public int type;
    public float x;
    public float y;
    public float rot;
    public float scaleX;
    public float scaleY;
}