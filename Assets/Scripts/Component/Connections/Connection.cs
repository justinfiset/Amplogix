using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;
using UnityEngine.UIElements;
using static Connection;

public class Connection : MonoBehaviour
{
    public enum ConnectionPosition { Left, Right, Top, Bottom }
    protected Connections connections;
    public bool HasVisibleConnections;
    private VisualConnection[] visualConnections = new VisualConnection[4];
    public GameObject visualConnectionPrefab;

    public static ConnectionPosition GetConnectionPositionFromIndex(int index)
    {
        switch (index)
        {
            case 0: return ConnectionPosition.Left;
            case 1: return ConnectionPosition.Right;
            case 2: return ConnectionPosition.Top;
            case 3: return ConnectionPosition.Bottom;
            default: throw new System.Exception("Position index must be between 0 and 3 = " + index);
        }
    }

    public int GetConnectionMultiplier(int index)
    {
        switch (index)
        {
            case 0: return 0;
            case 1: return 2;
            case 2: return 1;
            case 3: return 3;
            default: throw new System.Exception("Position index must be between 0 and 3 = " + index);
        }
    }

    public static int GetIndexFromPosition(ConnectionPosition connectionPosition)
    {
        switch (connectionPosition)
        {
            case ConnectionPosition.Left: return 0;
            case ConnectionPosition.Right: return 1;
            case ConnectionPosition.Top: return 2;
            case ConnectionPosition.Bottom: return 3;
            default: throw new System.Exception();
        }
    }

    public static ConnectionPosition GetOppositeConnection(ConnectionPosition connectionPosition)
    {
        switch (connectionPosition)
        {
            case ConnectionPosition.Left: return ConnectionPosition.Right;
            case ConnectionPosition.Right: return ConnectionPosition.Left;
            case ConnectionPosition.Top: return ConnectionPosition.Bottom;
            case ConnectionPosition.Bottom: return ConnectionPosition.Top;
            default: throw new System.Exception("value must not be null");
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        print("connection start");
        connections = new Connections();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    #region Visual Connections
    public void UpdateVisualConnections()
    {
        if (HasVisibleConnections)
        {
            for (int i = 0; i < 4; i++)
            {
                if (connections.GetValue(i)) // si connecté et pas de connection vis.
                {
                    if (visualConnections[i] == null)
                    {
                        CreateVisualConnection(i);
                    }
                } else if (visualConnections[i] != null) // si déco. et il y a co. vis.
                {
                    Destroy(visualConnections[i].gameObject);
                }
            }
        }
    }

    public void CreateVisualConnection(int index)
    {
        print("for " + gameObject + " at " + GetConnectionPositionFromIndex(index));
        GameObject instantiatedVisualConnection = Instantiate(visualConnectionPrefab,
            transform.position, Quaternion.Euler(0, 0, 90 * GetConnectionMultiplier(index)), transform);
        visualConnections[index] = instantiatedVisualConnection.GetComponent<VisualConnection>();
    }
    #endregion

    #region Deleting Connections
    public void DeleteAllLocalConnections()
    {
        connections.top = false; connections.bottom = false;
        connections.left = false; connections.right = false;

        UpdateVisualConnections();
    }

    public void DeleteLocalConnection(ConnectionPosition position)
    {
        switch (position)
        {
            case ConnectionPosition.Left: connections.left = false; break;
            case ConnectionPosition.Right: connections.right = false; break;
            case ConnectionPosition.Top: connections.top = false; break;
            case ConnectionPosition.Bottom: connections.bottom = false; break;
        }
        UpdateVisualConnections();
    }

    public void DeleteAllConnections()
    {
        for (int i = 0; i < 4; i++)
        {
            DeleteConnection(GetConnectionPositionFromIndex(i));
        }
    }

    public void DeleteConnection(ConnectionPosition position)
    {
        List<ElectricComponent> allConnections = GetAllConnectedTo();
        while (allConnections.Count < 4)
        {
            allConnections.Add(null);
        }
        allConnections[GetIndexFromPosition(position)].GetComponent<Connection>()
            .DeleteLocalConnection(GetOppositeConnection(position));
        DeleteLocalConnection(position);
    }
    #endregion

    #region ConnectTos
    public void ConnectTo(ConnectionPosition connectionPosition)
    {
        print("tried connecting " + gameObject);
        switch (connectionPosition)
        {
            case ConnectionPosition.Left:connections.left = true; break;
            case ConnectionPosition.Right:connections.right = true; break;
            case ConnectionPosition.Top:connections.top = true; break;
            case ConnectionPosition.Bottom:connections.bottom = true; break;
        }
        UpdateVisualConnections();
    }

    public void ConnectTo(int connectionPosition)
    {
        print("tried connecting " + gameObject + " with position " + GetConnectionPositionFromIndex(connectionPosition));
        switch (connectionPosition)
        {
            case 0: connections.left = true; break;
            case 1: connections.right = true; break;
            case 2: connections.top = true; break;
            case 3: connections.bottom = true; break;
            default: throw new System.Exception("Position index must be between 0 and 3 = " + connectionPosition);
        }
        UpdateVisualConnections();
    }
    #endregion

    #region Connection getters
    /*
     * Retourne un array de toutes les connection (Gauche, Droite, Haut, Bas)
     * Sauf celle reçue en argument
     * Null si il n'y a pas de connection à cet endroit
     */
    public List<ElectricComponent> GetConnectedToExcept(ElectricComponent entryPoint)
    {
        return GetConnectedTo(true, entryPoint);
    }
    /*
     * Retourne un array de toutes les connection(Gauche, Droite, Haut, Bas)
     * Null si il n'y a pas de connection à cet endroit
     */
    public List<ElectricComponent> GetAllConnectedTo()
    {
        return GetConnectedTo(false, null);
    }

    private List<ElectricComponent> GetConnectedTo(bool discardEntry, ElectricComponent entryPoint)
    {
        List<KeyValuePair<Vector2, ElectricComponent>> keyValuePairs;
        keyValuePairs = ProjectManager.m_Instance.GetSurroundingComponents(gameObject.transform.localPosition);

        List<ElectricComponent> result = new List<ElectricComponent>();

        for (int i = 0; i < keyValuePairs.Count; i++)
        {
            if (connections.GetValue(i))
            {
                ElectricComponent connectedComponent = keyValuePairs[i].Value;
                if (!discardEntry || connectedComponent != entryPoint)
                {
                    result.Add(connectedComponent);
                }
            }

        }
        return result;
    }
    #endregion

    public void OnDestroy()
    {
        DeleteAllConnections();
    }

    public class Connections
    {
        public bool left = false;
        public bool right = false;
        public bool top = false;
        public bool bottom = false;

        public bool GetValue(ConnectionPosition position)
        {
            switch (position)
            {
                case ConnectionPosition.Left:return left;
                case ConnectionPosition.Right:return right;
                case ConnectionPosition.Top:return top;
                case ConnectionPosition.Bottom:return bottom;
                default:return false;
            }
        }

        public bool GetValue(int position)
        {
            switch (position)
            {
                case 0: return left;
                case 1: return right;
                case 2: return top;
                case 3: return bottom;
                default: return false;
            }
        }
    }
}
