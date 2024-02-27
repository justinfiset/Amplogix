using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(SpriteRenderer))]
public class ResizeWingletBehaviour : MonoBehaviour
{
    private bool isHover;

    public ResizeWinglets manager;
    private SpriteRenderer sprite;

    private void Start()
    {
        sprite = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        if(isHover)
        {
            sprite.color = Color.gray;
            if (Input.GetMouseButton(0))
            {
                sprite.color = Color.yellow;
            }
        } else
        {
            sprite.color = Color.white;
        }
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
