using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class VisualConnection : MonoBehaviour
{
    SpriteRenderer sprite;

    public void Start()
    {
        sprite = GetComponent<SpriteRenderer>();
    }

    public void SetColor(Color color)
    {
        if(sprite != null)
        {
            sprite.color = color;
        }
    }
}
