using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConnectionTilesManager : MonoBehaviour
{
    private float spacing;
    private List<Vector2> surroundingComponents;
    private ElectricComponent currentComponent;

    public GameObject connectionTilePrefab;
    private GameObject parent;
    float rot; // the current component roatation

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ConnectComponents(ElectricComponent first, ElectricComponent second, bool horizontal)
    {
        //todo connecter des composants
    }

    #region Spawning and hiding tiles
    public void ShowTiles(ElectricComponent source)
    {
        parent = new GameObject("Connection Tiles");
        spacing = GridSettings.m_Instance.gridIncrement;
        rot = Mathf.Abs(transform.localEulerAngles.z);
        surroundingComponents = ProjectManager.m_Instance.GetSurroundingComponentsPos(transform.position);

        if (gameObject.GetComponent<ElectricComponent>().respectOrientation)
        {
            if (rot % 180 == 0) // Si horizontal,
            {
                SpawnHorizontalTiles(source);
            }
            else // Si vertical,
            {
                SpawnVerticalTiles(source);
            }
        }
        else
        {
            SpawnHorizontalTiles(source);
            SpawnVerticalTiles(source);
        }
    }

    public void HideTiles()
    {
        if (parent != null)
        {
            Destroy(parent);
        }
    }

    public void SpawnVerticalTiles(ElectricComponent source)
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

    public void SpawnHorizontalTiles(ElectricComponent source)
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
    #endregion
}
