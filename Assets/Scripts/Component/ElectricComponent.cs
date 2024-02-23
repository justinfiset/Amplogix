using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
[RequireComponent(typeof(SpriteRenderer))]
public class ElectricComponent : MonoBehaviour
{
    private bool isSelected;
    private bool isBeingHeld;

    public static KeyCode rotateKey = KeyCode.R;

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
                transform.Rotate(Vector3.forward * -45f);
            }

            // We snap the object according to the grid settings
            gameObject.transform.localPosition = GridSettings.GetCurrentSnapedPosition();
        }
    }

    private void OnSelect()
    {
        sprite.color = sprite.color * new Color(1, 1, 1, 0.5f);
    }

    private void OnUnselect()
    {
        sprite.color = Color.white;
    }

    private void OnMouseOver()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (isSelected)
            {
                isSelected = false;
                OnUnselect();
            }
            else
            {
                isSelected = true;
                OnSelect();
            }
        }
    }
}
