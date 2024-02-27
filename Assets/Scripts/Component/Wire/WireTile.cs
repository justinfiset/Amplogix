using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WireTile : MonoBehaviour
{
    private bool isHover = false;
    [HideInInspector] public ElectricComponent currentComponent; 

    public void Update()
    {
        if(isHover)
        {
            if(Input.GetMouseButtonDown(0))
            {
                ElectricComponentType type = ElectricComponentType.Wire;
                Quaternion angles = Quaternion.identity;
                // TODO GERER ORIENTATION

                GameObject component = ComponentSpawner.CreateComponent(type, transform.position, angles, Vector3.one);
                StartCoroutine(WaitBeforeSelection(component));
            }
        }
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
