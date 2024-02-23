using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEditor.TextCore.Text;

[Serializable]
[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(ResizeWinglets))]
public class ElectricComponent : MonoBehaviour
{
    public Vector3 origin;
    public Vector2 size; 

    private bool isSelected;

    public static KeyCode rotateKey = KeyCode.R;
    public static KeyCode deleteKey = KeyCode.Mouse2;

    private ResizeWinglets resizeWinglets;
    private SpriteRenderer sprite;

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

            // We snap the object according to the grid settings
            //gameObject.transform.localPosition = GridSettings.GetCurrentSnapedPosition();
        }
    }

    private void Select()
    {
        isSelected = true;
        OnSelect();

        origin = transform.position;
        size = transform.localScale;
        resizeWinglets.GenerateWinglets(origin, size);
    }

    private void Unselect()
    {
        isSelected = false;
        OnUnselect();
        resizeWinglets.DestroyWinglets();
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
            } else
            {
                Unselect();
            }
        }
    }
}
