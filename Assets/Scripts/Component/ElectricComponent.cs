using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(ResizeWinglets))]
[RequireComponent(typeof(WireTilesManager))]
[RequireComponent(typeof(BoxCollider2D))]
public class ElectricComponent : MonoBehaviour
{
    [SerializeField] protected ElectricComponentType type;
    private ElectricComponentData data;

    [Header("State")]
    private bool isHover = false;
    private bool isSelected = false;

    [Header("Move / Drag")]
    private bool hasReleasedSinceSelection = true;
    private bool isBeingMoved = false;
    private Vector3 startPos;
    private Vector3 startOrigin;

    [Header("UI")]
    private ResizeWinglets resizeWinglets;
    private WireTilesManager wireTilesManager;
    [SerializeField] private SpriteRenderer outline;
    private SpriteRenderer sprite;

    [Header("Inputs")]
    private static KeyCode rotateKey = KeyCode.R;
    private static KeyCode deleteKey = KeyCode.Mouse2;
    private static KeyCode unSelectKey = KeyCode.Escape;

    public virtual void RotateComponent()
    {
        transform.Rotate(Vector3.forward * -90);
    }

    public virtual void DestroyComponent()
    {
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
        if (isSelected)
        {
            if (Input.GetKeyDown(rotateKey))
            {
                RotateComponent();
            } 
            else if(Input.GetKeyDown(deleteKey))
            {
                DestroyComponent();
            } 
            else if(Input.GetKeyDown(unSelectKey))
            {
                Unselect();
            }

            if(!hasReleasedSinceSelection)
            {
                if(!isBeingMoved)
                {
                    startPos = GridSettings.GetCurrentSnapedPosition();
                    startOrigin = transform.localPosition;
                    isBeingMoved = true;
                }
                if(isBeingMoved)
                {
                    Vector3 diffPos = GridSettings.GetCurrentSnapedPosition() - startPos;
                    gameObject.transform.localPosition = startOrigin + diffPos;
                }
            }
        }

        if(Input.GetMouseButtonDown(0))
        {
            if (isHover)
            {
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


    }

    private void Select()
    {
        isSelected = true;
        resizeWinglets.GenerateWinglets(transform.localPosition, transform.localScale);
        wireTilesManager.ShowTiles();
        sprite.color = sprite.color * new Color(1, 1, 1, 0.5f);
        outline.color = Color.white;
    }

    private void Unselect()
    {
        isSelected = false;
        isBeingMoved = false;
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
    WireCorner
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