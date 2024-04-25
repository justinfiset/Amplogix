using MathNet.Numerics.RootFinding;
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
    public GameObject disconnectionTilePrefab;
    private GameObject parent;

    float rot;

    // Start is called before the first frame update
    void Start()
    {
        currentComponent = GetComponent<ElectricComponent>();
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
        ElectricComponent electricComponent = GetComponent<ElectricComponent>();

        if (gameObject.GetComponent<ElectricComponent>().respectOrientation)
        {
            if (rot % 180 == 0) // Si horizontal,
            {
                SpawnHorizontalConnectionTiles(electricComponent);
                SpawnHorizontalDisconnectionTiles(electricComponent);
            }
            else // Si vertical,
            {
                SpawnVerticalConnectionTiles(electricComponent);
                SpawnVerticalDisconnectionTiles(electricComponent);
            }
        }
        else
        {
            SpawnHorizontalConnectionTiles(electricComponent);
            SpawnVerticalConnectionTiles(electricComponent);
            SpawnHorizontalDisconnectionTiles(electricComponent);
            SpawnVerticalDisconnectionTiles(electricComponent);
        }
    }

    #region Spawning Connection Tiles
    public void SpawnVerticalConnectionTiles(ElectricComponent source)
    {
        if (!source.connectionManager.CanAddConnections()) return;

        Vector3 topPos = transform.position + (Vector3.up * spacing);
        Vector3 bottomPos = transform.position + (Vector3.down * spacing);
        Connection connectionComponent = GetComponent<Connection>();

        ElectricComponent topPosTarget = ProjectManager.GetComponent(topPos);
        if (ConnectionTileCheck(topPos, topPosTarget))
        {
            Instantiate(connectionTilePrefab, topPos, Quaternion.identity, parent.transform)
                .GetComponent<ConnectionTile>().Setup(this, TilePosition.Top, false, source);
        }

        ElectricComponent bottomPosTarget = ProjectManager.GetComponent(bottomPos);
        if (ConnectionTileCheck(bottomPos, bottomPosTarget))
        {
            Instantiate(connectionTilePrefab, bottomPos, Quaternion.identity, parent.transform)
                .GetComponent<ConnectionTile>().Setup(this, TilePosition.Bottom, false, source);
        }
    }

    public void SpawnHorizontalConnectionTiles(ElectricComponent source)
    {
        if (!source.connectionManager.CanAddConnections()) return;

        Vector3 leftPos = transform.position + (Vector3.left * spacing);
        Vector3 rightPos = transform.position + (Vector3.right * spacing);
        Connection connectionComponent = GetComponent<Connection>();

        ElectricComponent leftPosTarget = ProjectManager.GetComponent(leftPos);
        if (ConnectionTileCheck(leftPos, leftPosTarget))
        {
            Instantiate(connectionTilePrefab, leftPos, Quaternion.identity, parent.transform)
                .GetComponent<ConnectionTile>().Setup(this, TilePosition.Left, true, source);
        }

        ElectricComponent rightPosTarget = ProjectManager.GetComponent(rightPos);
        if (ConnectionTileCheck(rightPos, rightPosTarget))
        {
            Instantiate(connectionTilePrefab, rightPos, Quaternion.identity, parent.transform)
                .GetComponent<ConnectionTile>().Setup(this, TilePosition.Right, true, source);
        }
    }

    private bool ConnectionTileCheck(Vector3 pos, ElectricComponent target)
    {
        bool isNear = surroundingComponents.Contains(pos);
        Connection connection = GetComponent<Connection>();
        bool isConnected = isNear && connection.IsConnectedTo(target);
        if (isConnected)
        {
            return false;
        }

        bool canBeConnected = (target != null) && 
            ProjectManager.m_Instance.ComponentsPointToEachOther(currentComponent, target);
        return isNear && canBeConnected && target.connectionManager.CanAddConnections();
    }
    #endregion

    #region Spawning Disconnection Tiles
    public void SpawnVerticalDisconnectionTiles(ElectricComponent source)
    {
        Vector3 topPos = transform.position + (Vector3.up * spacing);
        Vector3 bottomPos = transform.position + (Vector3.down * spacing);
        Connection connectionComponent = GetComponent<Connection>();

        ElectricComponent topPosTarget = ProjectManager.GetComponent(topPos);
        if (DisconnectionTileCheck(topPos, topPosTarget))
        {
            Instantiate(disconnectionTilePrefab, topPos, Quaternion.identity, parent.transform)
                .GetComponent<DisconnectionTile>().Setup(this, TilePosition.Top, source, topPosTarget);
        }

        ElectricComponent bottomPosTarget = ProjectManager.GetComponent(bottomPos);
        if (DisconnectionTileCheck(bottomPos, bottomPosTarget))
        {
            Instantiate(disconnectionTilePrefab, bottomPos, Quaternion.identity, parent.transform)
                .GetComponent<DisconnectionTile>().Setup(this, TilePosition.Bottom, source, bottomPosTarget);
        }
    }

    public void SpawnHorizontalDisconnectionTiles(ElectricComponent source)
    {
        Vector3 leftPos = transform.position + (Vector3.left * spacing);
        Vector3 rightPos = transform.position + (Vector3.right * spacing);
        Connection connectionComponent = GetComponent<Connection>();

        ElectricComponent leftPosTarget = ProjectManager.GetComponent(leftPos);
        if (DisconnectionTileCheck(leftPos, leftPosTarget))
        {
            Instantiate(disconnectionTilePrefab, leftPos, Quaternion.identity, parent.transform)
                .GetComponent<DisconnectionTile>().Setup(this, TilePosition.Left, source, leftPosTarget);
        }

        ElectricComponent rightPosTarget = ProjectManager.GetComponent(rightPos);
        if (DisconnectionTileCheck(rightPos, rightPosTarget))
        {
            Instantiate(disconnectionTilePrefab, rightPos, Quaternion.identity, parent.transform)
                .GetComponent<DisconnectionTile>().Setup(this, TilePosition.Right, source, rightPosTarget);
        }
    }

    private bool DisconnectionTileCheck(Vector3 pos, ElectricComponent target)
    {
        bool isConnected = surroundingComponents.Contains(pos) && GetComponent<Connection>().IsConnectedTo(target);
        if (!isConnected)
        {
            return false;
        }

        bool forcedConnection = target.GetComponent<Connection>().ConnectsAutomaticallyToNeighbors &&
            GetComponent<Connection>().ConnectsAutomaticallyToNeighbors;
        return isConnected && !forcedConnection;
    }
    #endregion

    #region Showing Wire Tiles
    public void ShowWireTiles()
    {
        ElectricComponent component = GetComponent<ElectricComponent>();
        if(component.connectionManager.CanAddConnections())
        {
            spacing = GridSettings.m_Instance.gridIncrement;
            rot = Mathf.Abs(transform.localEulerAngles.z);
            surroundingComponents = ProjectManager.m_Instance.GetSurroundingComponentsPos(transform.position);

            if (component.respectOrientation)
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
    }

    public void SpawnVerticalWireTiles()
    {
        Vector3 topPos = transform.position + (Vector3.up * spacing);
        Vector3 bottomPos = transform.position + (Vector3.down * spacing);

        if (!surroundingComponents.Contains(topPos))
        {
            Instantiate(wireTilePrefab, topPos, Quaternion.identity, parent.transform)
                .GetComponent<WireTile>().Setup(this, TilePosition.Top);
        }
        if (!surroundingComponents.Contains(bottomPos))
        {
            Instantiate(wireTilePrefab, bottomPos, Quaternion.identity, parent.transform)
                .GetComponent<WireTile>().Setup(this, TilePosition.Bottom);
        }
    }

    public void SpawnHorizontalWireTiles()
    {
        Vector3 leftPos = transform.position + (Vector3.left * spacing);
        Vector3 rightPos = transform.position + (Vector3.right * spacing);

        if (!surroundingComponents.Contains(leftPos))
        {
            Instantiate(wireTilePrefab, leftPos, Quaternion.identity, parent.transform)
                .GetComponent<WireTile>().Setup(this, TilePosition.Left);
        }
        if (!surroundingComponents.Contains(rightPos))
        {
            Instantiate(wireTilePrefab, rightPos, Quaternion.identity, parent.transform)
                .GetComponent<WireTile>().Setup(this, TilePosition.Right);
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
