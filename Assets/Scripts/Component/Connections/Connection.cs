using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;
using UnityEngine.UIElements;
using static Connection;

public class Connection : MonoBehaviour
{
    public enum ConnectionPosition { Left = 0, Right = 1, Top = 2, Bottom = 3 }
    protected ConnectionsValue connections;
    public bool HasVisibleConnections;
    private VisualConnection[] visualConnections = new VisualConnection[4];
    public GameObject visualConnectionPrefab;

    public ElectricComponent[] GetConnectedComponents()
    {
        return connections.connections;
    }

    public bool IsConnected()
    {
        return connections.IsConnected();
    }

    public int ConnecitonCount()
    {
        return connections.ConnectionCount();
    }

    public static ConnectionPosition GetConnectionPositionFromIndex(int index)
    {
        return (ConnectionPosition)index;
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
        return (int)connectionPosition;
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

    void Start()
    {
        connections = new ConnectionsValue();
    }

    #region Visual Connections
    public void UpdateVisualConnections()
    {
        if (HasVisibleConnections)
        {
            for (int i = 0; i < 4; i++)
            {
                if (connections.IsConnected(i)) // si connecté et pas de connection vis.
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
        //print("for " + gameObject + " at " + GetConnectionPositionFromIndex(index));
        GameObject instantiatedVisualConnection = Instantiate(visualConnectionPrefab,
            transform.position, Quaternion.Euler(0, 0, 90 * GetConnectionMultiplier(index)), transform);
        visualConnections[index] = instantiatedVisualConnection.GetComponent<VisualConnection>();
    }
    #endregion

    #region Deleting Connections
    public void DeleteAllLocalConnections()
    {
        connections.DeleteAll();
        UpdateVisualConnections();
    }

    public void DeleteLocalConnection(ConnectionPosition position)
    {
        connections.SetValue((int)position, null);
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
        List<ElectricComponent> allConnected = GetAllConnectedTo();
        while (allConnected.Count < 4)
        {
            allConnected.Add(null);
        }

        ElectricComponent component = allConnected[GetIndexFromPosition(position)];
        if (component != null)
        {
            print(position);
            Connection connection = GetComponent<Connection>();
            connection.DeleteLocalConnection(GetOppositeConnection(position));
            DeleteLocalConnection(position);
        }
    }
    #endregion

    #region ConnectTos
    public void ConnectTo(ConnectionPosition connectionPosition, ElectricComponent component)
    {
        ConnectTo((int)connectionPosition, component);
    }

    public void ConnectTo(int connectionPosition, ElectricComponent component)
    {
        connections.SetValue(connectionPosition, component);
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
            if (connections.IsConnected(i))
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

    public class ConnectionsValue
    {
        public ElectricComponent[] connections = new ElectricComponent[4];

        public void DeleteAll()
        {
            connections = new ElectricComponent[connections.Length];
        }

        public void SetValue(int position, ElectricComponent value)
        {
            try
            {
                connections[position] = value;
            }
            catch
            {
                throw new System.Exception("Position index must be between 0 and 3 = " + position);
            }
        }

        public bool IsConnected(int position)
        {
            bool isConnected = false;
            try
            {
                isConnected = connections[position];
            } catch { }
            return isConnected;
        }

        public bool IsConnected()
        {
            foreach (bool connection in connections)
                if (connection) return true;
            return false;
        }

        public int ConnectionCount()
        {
            int count = 0;
            foreach (bool connection in connections)
                if(connection) count++;
            return count;
        }
    }
}
