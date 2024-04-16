using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightSource : MonoBehaviour
{
    public GameObject visualLightPrefab;
    [HideInInspector] public VisualLight visualLight;
    public float opacityMultiplier;
    public Color lightColor;
    private float intensity = 0f;

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

    public void SetIntensity(float intensity)
    {
        this.intensity = Mathf.Abs(intensity);
        OnIntensityChange();
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
        visualLight.SetOpacity(intensity);
        visualLight.SetOpacity(intensity * opacityMultiplier);
    }
}
