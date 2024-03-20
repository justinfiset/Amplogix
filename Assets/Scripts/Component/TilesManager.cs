using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TilesManager : MonoBehaviour
{
    [HideInInspector] public bool isActive;

    private float spacing;
    private List<Vector2> surroundingComponents;
    private ElectricComponent currentComponent;

    public GameObject wireTilePrefab;
    public GameObject connectionTilePrefab;
    private GameObject parent;

    float rot;

    // Start is called before the first frame update
    void Start()
    {
        currentComponent = GetComponent<ElectricComponent>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ShowTiles(ElectricComponent source)
    {
        parent = new GameObject("Tiles");
        ShowConnectionTiles(source);
        ShowWireTiles();
    }

    public void ShowConnectionTiles(ElectricComponent source)
    {
        spacing = GridSettings.m_Instance.gridIncrement;
        rot = Mathf.Abs(transform.localEulerAngles.z);
        surroundingComponents = ProjectManager.m_Instance.GetSurroundingComponentsPos(transform.position);

        if (gameObject.GetComponent<ElectricComponent>().respectOrientation)
        {
            if (rot % 180 == 0) // Si horizontal,
            {
                SpawnHorizontalConnectionTiles(gameObject.GetComponent<ElectricComponent>());
            }
            else // Si vertical,
            {
                SpawnVerticalConnectionTiles(gameObject.GetComponent<ElectricComponent>());
            }
        }
        else
        {
            SpawnHorizontalConnectionTiles(gameObject.GetComponent<ElectricComponent>());
            SpawnVerticalConnectionTiles(gameObject.GetComponent<ElectricComponent>());
        }
    }

    public void SpawnVerticalConnectionTiles(ElectricComponent source)
    {
        Vector3 topPos = transform.position + (Vector3.up * spacing);
        Vector3 bottomPos = transform.position + (Vector3.down * spacing);

        if (surroundingComponents.Contains(topPos))
        {
            Instantiate(connectionTilePrefab, topPos, Quaternion.identity, parent.transform)
                .GetComponent<ConnectionTile>().Setup(this, ConnectionTilePosition.Top, false, source);
        }
        if (surroundingComponents.Contains(bottomPos))
        {
            Instantiate(connectionTilePrefab, bottomPos, Quaternion.identity, parent.transform)
                .GetComponent<ConnectionTile>().Setup(this, ConnectionTilePosition.Bottom, false, source);
        }
    }

    public void SpawnHorizontalConnectionTiles(ElectricComponent source)
    {
        Vector3 leftPos = transform.position + (Vector3.left * spacing);
        Vector3 rightPos = transform.position + (Vector3.right * spacing);

        if (surroundingComponents.Contains(leftPos))
        {
            Instantiate(connectionTilePrefab, leftPos, Quaternion.identity, parent.transform)
                .GetComponent<ConnectionTile>().Setup(this, ConnectionTilePosition.Left, true, source);
        }
        if (surroundingComponents.Contains(rightPos))
        {
            Instantiate(connectionTilePrefab, rightPos, Quaternion.identity, parent.transform)
                .GetComponent<ConnectionTile>().Setup(this, ConnectionTilePosition.Right, true, source);
        }
    }

    #region Show wire tiles
    public void ShowWireTiles()
    {
        spacing = GridSettings.m_Instance.gridIncrement;
        rot = Mathf.Abs(transform.localEulerAngles.z);
        surroundingComponents = ProjectManager.m_Instance.GetSurroundingComponentsPos(transform.position);

        if (gameObject.GetComponent<ElectricComponent>().respectOrientation)
        {
            if (rot % 180 == 0) // Si horizontal,
            {
                SpawnHorizontalWireTiles();
            }
            else // Si vertical,
            {
                SpawnVerticalWireTiles();
            }
        }
        else
        {
            SpawnHorizontalWireTiles();
            SpawnVerticalWireTiles();
        }
    }

    public void SpawnVerticalWireTiles()
    {
        Vector3 topPos = transform.position + (Vector3.up * spacing);
        Vector3 bottomPos = transform.position + (Vector3.down * spacing);

        if (!surroundingComponents.Contains(topPos))
        {
            Instantiate(wireTilePrefab, topPos, Quaternion.identity, parent.transform)
                .GetComponent<WireTile>().Setup(this, WireTilePosition.Top);
        }
        if (!surroundingComponents.Contains(bottomPos))
        {
            Instantiate(wireTilePrefab, bottomPos, Quaternion.identity, parent.transform)
                .GetComponent<WireTile>().Setup(this, WireTilePosition.Bottom);
        }
    }

    public void SpawnHorizontalWireTiles()
    {
        Vector3 leftPos = transform.position + (Vector3.left * spacing);
        Vector3 rightPos = transform.position + (Vector3.right * spacing);

        if (!surroundingComponents.Contains(leftPos))
        {
            Instantiate(wireTilePrefab, leftPos, Quaternion.identity, parent.transform)
                .GetComponent<WireTile>().Setup(this, WireTilePosition.Left);
        }
        if (!surroundingComponents.Contains(rightPos))
        {
            Instantiate(wireTilePrefab, rightPos, Quaternion.identity, parent.transform)
                .GetComponent<WireTile>().Setup(this, WireTilePosition.Right);
        }
    }
    #endregion

    public void CreateNewWire(WireTile tile)
    {
        ElectricComponentType type = ElectricComponentType.Wire;
        Vector3 pos = tile.transform.position;
        Quaternion angles = Quaternion.Euler(0, 0, (float)tile.position);
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
        if (parent != null)
        {
            Destroy(parent);
        }
    }
}
