using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using UnityEditor;
using UnityEngine.EventSystems;

//[RequireComponent(typeof(SpriteRenderer))]
//[RequireComponent(typeof(ResizeWinglets))]
//[RequireComponent(typeof(WireTilesManager))]
[RequireComponent(typeof(BoxCollider2D))]
public class ElectricComponent : MonoBehaviour
{
    [Header("Logic")]
    public ElectricComponentType type;
    public bool canBeRotated = true; // Le composant peut-il être rotationé par l'utilisateur?
    public bool canBeMoved = true; // Le composant peut-il être manipulé par l'utilisateur?
    public bool canStack = false;
    public bool snapToGrid = true;
    public bool respectOrientation = false;

    [Header("State")]
    protected bool isHover = false;
    protected bool isSelected = false;
    protected bool listenToInputs = true;
    protected bool isMouseOverGUI = false;
    [HideInInspector] public string initialComponentData = "";

    [Header("Move / Drag")]
    [HideInInspector] public bool hasReleasedSinceSelection = true;
    protected bool isBeingMoved = false;
    protected Vector3 lastClickMousePos;
    protected Vector3 lastClickPos;

    [Header("UI")]
    [SerializeField] protected SpriteRenderer outline;
    private ResizeWinglets resizeWinglets;
    private WireTilesManager wireTilesManager;
    private ConnectionTilesManager connectionTilesManager;
    private SpriteRenderer sprite;

    [Header("Inputs")]
    private static KeyCode rotateKey = KeyCode.R;
    private static KeyCode mouseDeleteKey = KeyCode.Mouse2;
    private static KeyCode keyboardDeleteKey = KeyCode.Backspace;
    private static KeyCode systemDeleteKey = KeyCode.Delete;
    private static KeyCode unselectKey = KeyCode.Escape;
    private static KeyCode interactKey = KeyCode.Mouse1;

    private void Start()
    {
        resizeWinglets = GetComponent<ResizeWinglets>();
        wireTilesManager = GetComponent<WireTilesManager>();
        connectionTilesManager = GetComponent<ConnectionTilesManager>();
        sprite = GetComponent<SpriteRenderer>();

        Setup();
        if(initialComponentData != "")
        {
            UnpackCustomComponentData(initialComponentData);
        }
    }

    void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            if (isHover)
            {
                if(snapToGrid)
                {
                    lastClickMousePos = GridSettings.GetCurrentSnapedPosition();
                } else
                {
                    lastClickPos = transform.position;
                    lastClickMousePos = GridSettings.MouseInputToWorldPoint();
                }

                if (!isSelected)
                {
                    _Select();
                    hasReleasedSinceSelection = false;
                }
            }
        }
        else if(Input.GetMouseButtonUp(0))
        {
            if (isSelected && hasReleasedSinceSelection && !isMouseOverGUI)
            {
                // TODO verif si fonctionne bien TEST
                if(!EventSystem.current.IsPointerOverGameObject())
                {
                    _Unselect();
                }
            } else
            {
                hasReleasedSinceSelection = true;
            }
        }

        if(isHover)
        {
            if (Input.GetKeyDown(interactKey))
            {
                Interact();
            }
        }

        if (isSelected)
        {
            if(listenToInputs)
            {
                if (canBeRotated && Input.GetKeyDown(rotateKey))
                {
                    _RotateComponent();
                }
                else if (Input.GetKeyDown(mouseDeleteKey)
                    || Input.GetKeyDown(keyboardDeleteKey)
                    || Input.GetKeyDown(systemDeleteKey))
                {
                    _DestroyComponent();
                }
                else if (Input.GetKeyDown(unselectKey))
                {
                    _Unselect();
                }
            }

            if (!hasReleasedSinceSelection)
            {
                if (!isBeingMoved)
                {
                    isBeingMoved = true;
                }

                if (isBeingMoved && canBeMoved)
                {
                    Vector3 newPos;
                    if (snapToGrid)
                    {
                        newPos = GridSettings.GetCurrentSnapedPosition();
                    } 
                    else
                    {
                        Vector3 diffPos = lastClickMousePos - GridSettings.MouseInputToWorldPoint();
                        newPos = lastClickPos - diffPos;
                    }
                    MoveComponent(newPos);
                }
            }
            else // Si on relache le boutton
            {
                if (!canStack && isBeingMoved)
                {
                    if (ProjectManager.m_Instance.GetComponentCount(transform.position) > 1)
                    {
                        MoveComponent(lastClickPos);
                    }
                }
            }
        }

        OnUpdate(); // pour les sous composants
    }

    #region Internal
    public void MoveComponent(Vector3 newPos)
    {
        if(newPos != transform.position)
        {
            _Unselect();
            transform.position = newPos;
            ProjectManager.m_Instance.ChangeComponentPos(this, transform.position);
            ProjectManager.OnModifyProject();
            _Select();
        }
    }

    public void _Select()
    {
        isSelected = true;
        Select();
        outline.color = Color.white;
        ProjectManager.AddComponentToSelection(this);
    }

    public void _Unselect()
    {
        isSelected = false;
        Unselect();
        outline.color = Color.clear;
        ProjectManager.RemoveComponentFromSelection(this);
    }

    private void _RotateComponent()
    {
        _Unselect();
        RotateComponent();
        _Select();
        ProjectManager.OnModifyProject();
    }

    public void _DestroyComponent()
    {
        _Unselect();
        DestroyComponent();
        ComponentSpawner.DestroyComponent(gameObject);
    }
    #endregion

    #region Inheritance
    public virtual void Interact() { }

    public virtual void Setup() { }

    public virtual void OnUpdate() { }

    public virtual void DestroyComponent() { }

    public virtual void RotateComponent() { 
        transform.Rotate(Vector3.forward * -90); 
    }

    public virtual void UnpackCustomComponentData(string customDataString) { return; }

    public virtual string GetCustomComponentData() { return ""; }

    public virtual void Select()
    {
        resizeWinglets.GenerateWinglets(transform.position, transform.localScale);
        wireTilesManager.ShowTiles();
        connectionTilesManager.ShowTiles(this);
        sprite.color = sprite.color * new Color(1, 1, 1, 0.5f);
    }

    public virtual void Unselect()
    {
        resizeWinglets.DestroyWinglets();
        wireTilesManager.HideTiles();
        connectionTilesManager.HideTiles();
        sprite.color = Color.white;
    }
    #endregion

    #region Data / Serialization / Unpack
    public ElectricComponentData GetData()
    {
        return new ElectricComponentData(this);
    }

    public T UnserializeCustomComponentData<T>(string customDataString)
    {
        return JsonUtility.FromJson<T>(customDataString);
    }

    public string SerializeCustomComponentData<T>(T customDataClass)
    {
        return JsonUtility.ToJson(customDataClass);
    }
    #endregion

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

#region Component Type
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
    WireFourWay,
    TextLabel = 99
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
#endregion

#region Component Data
[Serializable]
public class ElectricComponentData
{
    public int type;
    public float x;
    public float y;
    public float rot;
    public float scaleX;
    public float scaleY;
    public string customComponentData;

    public ElectricComponentData(ElectricComponent component)
    {
        type = (int)component.type;
        x = component.transform.position.x;
        y = component.transform.position.y;
        rot = component.transform.localEulerAngles.z;
        scaleX = component.transform.localScale.x;
        scaleY = component.transform.localScale.y;
        customComponentData = component.GetCustomComponentData();
    }
}
#endregion