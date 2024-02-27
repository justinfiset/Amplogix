using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WireTilesManager : MonoBehaviour
{
    [HideInInspector] public bool isActive;

    public bool respectOrientation;
    private float spacing;

    public GameObject wireTilePrefab;
    private GameObject parent;

    private void Update()
    {
       if(isActive)
        {

        }
    }

    public void ShowTiles()
    {
        parent = new GameObject("Wire Tiles");
        spacing = GridSettings.m_Instance.gridIncrement;
        // TODO checkup case non dispo
        float rot = Mathf.Abs(transform.localEulerAngles.z);

        if(respectOrientation )
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
        Instantiate(wireTilePrefab, topPos, Quaternion.identity, parent.transform);
        Instantiate(wireTilePrefab, bottomPos, Quaternion.identity, parent.transform);
    }

    public void SpawnHorizontalTiles()
    {
        Vector3 leftPos = transform.position + (Vector3.left * spacing);
        Vector3 rightPos = transform.position + (Vector3.right * spacing);
        Instantiate(wireTilePrefab, leftPos, Quaternion.identity, parent.transform);
        Instantiate(wireTilePrefab, rightPos, Quaternion.identity, parent.transform);
    }

    public void HideTiles()
    {
        if(parent != null)
        {
            Destroy(parent);
        }
    }
}
