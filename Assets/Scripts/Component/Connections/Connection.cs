using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEditor.PlayerSettings;

public class Connection : MonoBehaviour
{
    public enum Position { Left = 0, Right = 1, Top = 2, Bottom = 3 }

    public ConnectionsValue connections { get; private set; }
    public bool HasVisibleConnections;
    private VisualConnection[] visualConnections = new VisualConnection[4];
    public GameObject visualConnectionPrefab;
    public bool ConnectsAutomaticallyToNeighbors;

    public void CreateInitialConnections(ConnectionValueData data)
    {
        for(int i = 0; i < data.connections.Length; i++)
        {
            if (data.connections[i])
            {
                ConnectTo(i);
            }
        }
    }

    public void SetColor(Color newColor)
    {
        foreach(VisualConnection c in visualConnections)
        {
            if(c != null)
            {
                c.SetColor(newColor);
            }
        }
    }

    public ElectricComponent[] GetConnectedComponents()
    {
        return connections.connections;
    }

    public bool IsConnected()
    {
        return connections.IsConnected();
    }

    public bool IsConnectedTo(ElectricComponent electricComponent)
    {
        return connections.connections.Contains(electricComponent);
    }

    public int ConnectionCount()
    {
        return connections.ConnectionCount();
    }

    public static Position GetConnectionPositionFromIndex(int index)
    {
        return (Position)index;
    }

    public int GetConnectionMultiplier(int index)
    {
        switch (index)
        {
            case 0: return 0;
            case 1: return 2;
            case 2: return 3;
            case 3: return 1;
            default: throw new System.Exception("Position index must be between 0 and 3 = " + index);
        }
    }

    public static int GetIndexFromPosition(Position connectionPosition)
    {
        return (int)connectionPosition;
    }

    public static Position GetOppositeConnection(Position connectionPosition)
    {
        switch (connectionPosition)
        {
            case Position.Left: return Position.Right;
            case Position.Right: return Position.Left;
            case Position.Top: return Position.Bottom;
            case Position.Bottom: return Position.Top;
            default: throw new System.Exception("value must not be null");
        }
    }

    void Start()
    {
        connections = new ConnectionsValue();
        StartCoroutine(WaitForAutoConnections());
    }

    public IEnumerator WaitForAutoConnections()
    {
        yield return new WaitForEndOfFrame();
        AutoConnect();
    }

    public void AutoConnect()
    {
        if (ConnectsAutomaticallyToNeighbors)
        {
            ElectricComponent electricComponent = GetComponent<ElectricComponent>();
            List<KeyValuePair<Vector2, ElectricComponent>> surroundingComps;
            surroundingComps = ProjectManager.m_Instance.GetSurroundingComponents(transform.position);

            foreach (KeyValuePair<Vector2, ElectricComponent> k in surroundingComps)
            {
                ProjectManager.m_Instance.ConnectComponents(electricComponent, k.Value);
            }
        }
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

    public IEnumerator WaitToUpdateVisualConnections()
    {
        yield return new WaitForEndOfFrame();

        UpdateVisualConnections();
    }

    public ConnectionValueData GetData()
    {
        return new ConnectionValueData(this);
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

    public void DeleteLocalConnection(Position position)
    {
        DeleteLocalConnection((int)position);
    }

    public void DeleteLocalConnection(int position)
    {
        connections.SetValue(position, null);
        DoWireCheck();
        UpdateVisualConnections();
    }

    public void DeleteLocalConnection(ElectricComponent component)
    {
        for (int i = 0; i < connections.connections.Count(); i++)
        {
            if (connections.connections[i] == component)
            {
                connections.connections[i] = null;
                break;
            }
        }
        DoWireCheck();
        UpdateVisualConnections();
    }

    private void DoWireCheck()
    {
        ElectricComponent electricComponent = gameObject.GetComponent<ElectricComponent>();
        if (electricComponent.type == ElectricComponentType.Wire)
        {
            if (!HasActiveConnections())
            {
                electricComponent._DestroyComponent();
            }
        }
    }

    public bool HasActiveConnections()
    {
        foreach (ElectricComponent connection in connections.connections)
        {
            if (connection != null)
            {
                return true;
            }
        }
        return false;
    }

    public void DeleteAllConnections()
    {
        foreach (ElectricComponent component in connections.connections)
        {
            DeleteConnection(component);
        }
    }

    public void DeleteConnection(ElectricComponent target)
    {
        if(target != null)
        {
            Connection targetConnection = target.GetComponent<Connection>();

            targetConnection.DeleteLocalConnection(GetComponent<ElectricComponent>());
            DeleteLocalConnection(target);
        }
    }

    public void DeleteConnection(Position position)
    {
        List<ElectricComponent> allConnected = GetAllConnectedTo();


        if (GetIndexFromPosition(position) >= allConnected.Count) return;

        ElectricComponent component = allConnected[GetIndexFromPosition(position)];
        if (component != null)
        {
            //print(position);
            Connection connection = component.GetComponent<Connection>();
            connection.DeleteLocalConnection(GetOppositeConnection(position));
            DeleteLocalConnection(position);
        }
    }
    #endregion

    #region ConnectTos
    private void ConnectTo(Position connectionPosition, ElectricComponent component)
    {
        ConnectTo((int)connectionPosition, component);
    }

    public void ConnectTo(int connectionPosition)
    {
        List<KeyValuePair<Vector2, ElectricComponent>> list = 
            ProjectManager.m_Instance.GetSurroundingComponentsWithNulls(transform.position);

        ConnectTo(connectionPosition, list[connectionPosition].Value);
    }

    public void ConnectTo(Position connectionPosition)
    {
        ConnectTo((int)connectionPosition);
    }

    private void ConnectTo(int connectionPosition, ElectricComponent component)
    {
        connections.SetValue(connectionPosition, component);
        UpdateVisualConnections();
        ProjectManager.OnModifyProject(ProjectModificationType.CircuitModification);
    }
    public void ConnectTo(ElectricComponent component)
    {
        // TODO simplified ConnectTo
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
        List<ElectricComponent> result = new(connections.connections);

        if (discardEntry)
        {
            foreach (ElectricComponent e in result)
            {
                if (e == entryPoint)
                {
                    result.Remove(e);
                }
            }
        }

        return result;
    }
    #endregion

    [Serializable]
    public class ConnectionValueData
    {
        public bool[] connections = new bool[4];

        public ConnectionValueData(Connection value)
        {
            var indexList = Enum.GetValues(typeof(Position));
            foreach (Position position in indexList)
            {
                int index = (int)position;
                this.connections[index] = (value.connections.connections[index] != null);
            }
        }
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
