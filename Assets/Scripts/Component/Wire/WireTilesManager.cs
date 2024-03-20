using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public class WireTilesManager : MonoBehaviour
{
    [HideInInspector] public bool isActive;

    private float spacing;
    private List<Vector2> surroundingComponents;
    private ElectricComponent currentComponent;

    public GameObject wireTilePrefab;
    private GameObject parent;

    float rot; // the current component roatation

    private void Start()
    {
        currentComponent = GetComponent<ElectricComponent>();
    }

    public void ShowTiles()
    {
        parent = new GameObject("Wire Tiles");
        spacing = GridSettings.m_Instance.gridIncrement;
        rot = Mathf.Abs(transform.localEulerAngles.z);
        surroundingComponents = ProjectManager.m_Instance.GetSurroundingComponentsPos(transform.position);

        if(gameObject.GetComponent<ElectricComponent>().respectOrientation)
        {
            if (rot % 180 == 0) // Si horizontal,
            {
                SpawnHorizontalTiles();
            }
            else // Si vertical,
            {
                SpawnVerticalTiles();
            }
        } else
        {
            SpawnHorizontalTiles();
            SpawnVerticalTiles();
        }
    }

    public void SpawnVerticalTiles()
    {
        Vector3 topPos = transform.position + (Vector3.up * spacing);
        Vector3 bottomPos = transform.position + (Vector3.down * spacing);

        if(!surroundingComponents.Contains(topPos))
        {
            // Instantiate(wireTilePrefab, topPos, Quaternion.identity, parent.transform)
            //     .GetComponent<WireTile>().Setup(this, WireTilePosition.Top);
        }
        if (!surroundingComponents.Contains(bottomPos))
        {
            // Instantiate(wireTilePrefab, bottomPos, Quaternion.identity, parent.transform)
            //     .GetComponent<WireTile>().Setup(this, WireTilePosition.Bottom);
        }
    }

    public void SpawnHorizontalTiles()
    {
        Vector3 leftPos = transform.position + (Vector3.left * spacing);
        Vector3 rightPos = transform.position + (Vector3.right * spacing);

        if (!surroundingComponents.Contains(leftPos))
        {
            // Instantiate(wireTilePrefab, leftPos, Quaternion.identity, parent.transform)
            //     .GetComponent<WireTile>().Setup(this, WireTilePosition.Left);
        }
        if (!surroundingComponents.Contains(rightPos))
        {
            // Instantiate(wireTilePrefab, rightPos, Quaternion.identity, parent.transform)
            //     .GetComponent<WireTile>().Setup(this, WireTilePosition.Right);
        }
    }

    public void CreateNewWire(WireTile tile)
    {
        ElectricComponentType type = ElectricComponentType.Wire;
        Vector3 pos = tile.transform.position;
        Quaternion angles = Quaternion.Euler(0, 0, (float) tile.position);
        GameObject component = ComponentSpawner.CreateComponent(type, pos, angles, Vector3.one, Color.black).gameObject;
        
        StartCoroutine(WaitAndConnectTo(component));
        ManageNewSelection(component);
        // ProjectManager.m_Instance.ConnectComponents(component.GetComponent<ElectricComponent>(), 
        //     tile.GetComponentInParent<ElectricComponent>());
        //HandleWireVariation(component);
    }

    private IEnumerator WaitAndConnectTo(GameObject component)
    {
        yield return new WaitForEndOfFrame();
        ProjectManager.m_Instance.ConnectComponents(component.GetComponent<ElectricComponent>(),
            GetComponent<ElectricComponent>());
    }

    public void HandleWireVariation(GameObject newWire)
    {
        // On modifie la variation uniquement si on est dans un fil
        if(ElectricComponentTypeMethods.IsWire(currentComponent))
        {
            surroundingComponents = ProjectManager.m_Instance.GetSurroundingComponentsPos(transform.position);

            // TODO SCAN COMPONENTS AUTOUR POUR LIER AU BESOIN
        }
    }

    public void ManageNewSelection(GameObject newWire)
    {
        StartCoroutine(WaitBeforeSelection(newWire));
    }

    private IEnumerator WaitBeforeSelection(GameObject newWire)
    {
        yield return new WaitForEndOfFrame();
        currentComponent._Unselect();
        ElectricComponent wire = newWire.GetComponent<ElectricComponent>();
        if (wire != null)
        {
            wire._Select();
            wire.hasReleasedSinceSelection = false;
        }
    }

    public void HideTiles()
    {
        if(parent != null)
        {
            Destroy(parent);
        }
    }
}
