using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightSource : MonoBehaviour
{
    public GameObject visualLightPrefab;
    [HideInInspector] public VisualLight visualLight;
    public float opacityMultiplier;
    public Color lightColor;

    public bool doLighting;

    // Start is called before the first frame update
    void Start()
    {
        CreateLight();
        if (!doLighting)
        {
            visualLight.gameObject.SetActive(false);
        }
    }

    private void CreateLight()
    {
        visualLight = Instantiate(visualLightPrefab, transform).GetComponent<VisualLight>();
        Vector3 tmp = visualLight.transform.position;
        tmp.z += 1;
        visualLight.transform.position = tmp;
        visualLight.SetRGBColor(lightColor);
        UpdateLighting();
    }

    public void OnIntensityChange()
    {
        if (doLighting)
        {
            UpdateLighting();
        }
    }

    public void UpdateLighting()
    {
        visualLight.SetOpacity(GetIntensity() * opacityMultiplier);
    }

    private float GetIntensity()
    {
        return 255f; //TODO: changer la méthode pour get la vraie intensité
    }
}
