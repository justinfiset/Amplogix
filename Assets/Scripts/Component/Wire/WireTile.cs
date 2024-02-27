using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WireTile : MonoBehaviour
{
    private bool isHover = false;
    private float spawnRot = 0;
    private ElectricComponent currentComponent; 

    public void Update()
    {
        if(isHover)
        {
            if(Input.GetMouseButtonDown(0))
            {
                ElectricComponentType type = ElectricComponentType.Wire;
                Quaternion angles = Quaternion.Euler(0, 0, spawnRot);
                // TODO GERER COIN <---
                GameObject component = ComponentSpawner.CreateComponent(type, transform.position, angles, Vector3.one);
                StartCoroutine(WaitBeforeSelection(component));
            }
        }
    }

    public void Setup(ElectricComponent component, float spawnRot)
    {
        this.currentComponent = component;
        this.spawnRot = spawnRot;
    }

    private IEnumerator WaitBeforeSelection(GameObject component)
    {
        yield return new WaitForEndOfFrame();
        currentComponent.Unselect();
        ElectricComponent wire = component.GetComponent<ElectricComponent>();
        if (wire != null) {
            wire.Select();
            wire.hasReleasedSinceSelection = false;
        }
    }

    #region Mouse callbacks
    private void OnMouseEnter()
    {
        isHover = true;
    }

    private void OnMouseExit()
    {
        isHover = false;
    }
    #endregion
}
