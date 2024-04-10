using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightSource : MonoBehaviour
{
    public GameObject visualLightPrefab;
    [HideInInspector] public VisualLight visualLight;
    public int opacityMultiplier;
    public Color lightColor;

    // Start is called before the first frame update
    void Start()
    {
        visualLight = Instantiate(visualLightPrefab, transform).GetComponent<VisualLight>();
        visualLight.SetRGBColor(lightColor);
        UpdateLighting();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnIntensityChange()
    {
        UpdateLighting();
    }

    public void UpdateLighting()
    {
        visualLight.SetOpacity(GetIntensity());
    }

    private float GetIntensity()
    {
        return 255f; //TODO: changer la méthode pour get la vraie intensité
    }
}
