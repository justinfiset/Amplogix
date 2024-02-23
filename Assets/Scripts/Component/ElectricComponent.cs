using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEditor.TextCore.Text;

[Serializable]
[RequireComponent(typeof(SpriteRenderer))]
public class ElectricComponent : MonoBehaviour
{
    public bool isSelected;
    public bool isBeingHeld;
    public bool canHold;

    public static KeyCode rotateKey = KeyCode.R;
    public static KeyCode deleteKey = KeyCode.Mouse2;

    private SpriteRenderer sprite;

    void Start()
    {
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

            if(canHold)
            {
                if(Input.GetMouseButtonDown(0))
                {
                    isBeingHeld = true;
                }
                else if(Input.GetMouseButtonUp(0))
                {
                    isBeingHeld = false;
                    Unselect();
                    canHold = false;
                }
            } 
            else
            {
                if (Input.GetMouseButtonUp(0))
                {
                    canHold = true;
                }
            }

            // We snap the object according to the grid settings
            if (isBeingHeld)
            {
                gameObject.transform.localPosition = GridSettings.GetCurrentSnapedPosition();
            }
        }
    }

    private void Select()
    {
        isSelected = true;
        OnSelect();
    }

    private void Unselect()
    {
        isSelected = false;
        OnUnselect();
    }

    private void OnSelect()
    {
        sprite.color = sprite.color * 0.5f;
    }

    private void OnUnselect()
    {
        sprite.color = sprite.color * new Color(1, 1, 1, 0.5f);
    }

    private void OnMouseOver()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (!isSelected)
            {
                Select();
            }
        }
    }
}
