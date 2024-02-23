using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEditor.TextCore.Text;

[Serializable]
[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(ResizeWinglets))]
[RequireComponent(typeof(BoxCollider2D))]
public class ElectricComponent : MonoBehaviour
{
    [Header("State")]
    public bool isHover = false;
    public bool isSelected = false;

    [Header("Move / Drag")]
    private bool hasReleasedSinceSelection = true;
    private bool isBeingMoved = false;
    private Vector3 startPos;
    private Vector3 startOrigin;

    [Header("UI")]
    private ResizeWinglets resizeWinglets;
    private SpriteRenderer sprite;

    [Header("Inputs")]
    public static KeyCode rotateKey = KeyCode.R;
    public static KeyCode deleteKey = KeyCode.Mouse2;
    public static KeyCode unSelectKey = KeyCode.Escape;

    void Start()
    {
        resizeWinglets = GetComponent<ResizeWinglets>();
        sprite = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        if (isSelected)
        {
            if (Input.GetKeyDown(rotateKey))
            {
                transform.Rotate(Vector3.forward * -90);
            } 
            else if(Input.GetKeyDown(deleteKey))
            {
                ComponentSpawner.DestroyComponent(gameObject);
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
        OnSelect();

        resizeWinglets.GenerateWinglets(transform.localPosition, transform.localScale);
    }

    private void Unselect()
    {
        isSelected = false;
        isBeingMoved = false;
        resizeWinglets.DestroyWinglets();
        OnUnselect();
    }

    private void OnSelect()
    {
        sprite.color = sprite.color * new Color(1, 1, 1, 0.5f);
    }

    private void OnUnselect()
    {
        sprite.color = Color.white;
    }

    private void OnMouseEnter()
    {
        isHover = true;
    }

    private void OnMouseExit()
    {
        isHover = false;
    }
}
