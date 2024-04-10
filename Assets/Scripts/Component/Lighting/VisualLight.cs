using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisualLight : MonoBehaviour
{
    public Color color;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetRGBColor(Color newColor)
    {
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        Color tempColor = spriteRenderer.color;

        tempColor.r = newColor.r;
        tempColor.g = newColor.g;
        tempColor.b = newColor.b;

        spriteRenderer.color = tempColor;
    }

    public void SetOpacity(float opacity) 
    {
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        Color tempColor = spriteRenderer.color;

        tempColor.a = opacity;

        spriteRenderer.color = tempColor;
    }
}
