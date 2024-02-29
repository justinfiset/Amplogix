using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElectricSwitch : ElectricComponent
{
    private SpriteRenderer spriteRenderer;

    public Sprite closedSprite;
    public Sprite openSprite;

    public bool isOpen = true;

    override public void Setup()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    override public void Interact()
    {
        isOpen = !isOpen;
        spriteRenderer.sprite = isOpen ? openSprite : closedSprite;
    }
}
