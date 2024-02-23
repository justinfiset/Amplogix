using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridSettings : MonoBehaviour
{
    public float gridIncrement = 1; // the step between two grid position (x and y axis)

    public static GridSettings m_Instance { get; private set; } // the Singleton instance, only usable by the class itself
    /// <summary>
    /// Singleton creation and making sure only one instance exists
    /// </summary>
    private void Awake()
    {
        if (m_Instance == null)
            m_Instance = this;
        else if(m_Instance != this) 
            Destroy(this);
    }

    /// <summary>
    /// Calculate the X and Y coordinates of the mouse in the world
    /// </summary>
    /// <returns>A vector with the X and Y pos of the mouse in world scale</returns>
    public static Vector3 MouseInputToWorldPoint()
    {
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        worldPos.Scale(new Vector3(1, 1, 0));
        return worldPos;
    }

    /// <summary>
    /// Returns the current mouse position snapped to the nearest position
    /// relativeto the grid size.
    /// ex: if bound = 5: 4->5, 11->10, 13->15;
    /// </summary>
    /// <returns>The nearest point in space that is part of the grid system</returns>
    public static Vector3 GetCurrentSnapedPosition()
    {
        Vector3 snappedPos;
        Vector3 gridPos = MouseInputToWorldPoint();
        gridPos.x = Mathf.Round(gridPos.x / m_Instance.gridIncrement);
        gridPos.y = Mathf.Round(gridPos.y / m_Instance.gridIncrement);
        snappedPos = gridPos * m_Instance.gridIncrement;

        return snappedPos;
    }
}
    