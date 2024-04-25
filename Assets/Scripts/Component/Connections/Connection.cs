using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class Connection : MonoBehaviour
{
    public int maxConnectionCount = 2;

    public enum Position { Left = 0, Right = 1, Top = 2, Bottom = 3 }

    public ConnectionsValue connections { get; private set; }
    public bool HasVisibleConnections;
    private VisualConnection[] visualConnections = new VisualConnection[4];
    public GameObject visualConnectionPrefab;
    public bool ConnectsAutomaticallyToNeighbors;

    public bool CanAddConnections()
    {
        // Un fil n'a pas de limite de connection (pas de limite fixe / limit� par les cases autours)
        if (GetComponent<ElectricComponent>().type == ElectricComponentType.Wire)
            return true;
        else
            return ConnectionCount() < maxConnectionCount;
    }
    public int WhereIsConnected(ElectricComponent component)
    {
        for (int i = 0; i < connections.connections.Length; i++)
        {
            if (connections.connections[i] == component)
            {
                return i;
            }
        }
        throw new Exception("not connected to " + component);
    }

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

    public bool IsFlatConnection()
    {
        if (GetNumberOfConnections() != 2)
        {
            //print("component doesnt have 2 connections");
            return false;
        }

        //print("connections from " + gameObject);
        foreach (ElectricComponent c in connections.connections)
        {
            //print(c);
        }

        for (int i = 0; i < 4; i++)
        {
            /*
            print("for " + connections.connections[i] + " at i = " + i);
            print((Position)i);
            print("opposite: " + GetOppositeConnection((Position)i));
            print("opposite is " + GetOppositeComponent(connections.connections[i]));
            
            if (connections.connections[i] == null) {
                print("checked connection is null");
            }
            */
            
            //print(GetOppositeComponent(connections.connections[i]) == null);
            
            if (connections.connections[i] != null && GetOppositeComponent(connections.connections[i]) != null)
            {
                //print("connection is flat");
                return true;
            }
        }
        //print("no parallel connections found");
        return false;
    }

    public int GetNumberOfConnections()
    {
        int count = 0;
        foreach (ElectricComponent c in connections.connections)
        {
            if (c != null)
            {
                count++;
            }
        }
        return count;
    }

    public ElectricComponent GetOppositeComponent(ElectricComponent input)
    {
        for (int i = 0; i <= connections.connections.Length; i++)
        {
            ElectricComponent c = connections.connections[i];
            if (c == input)
            {
                return connections.connections[(int)GetOppositeConnection((Position)i)];
            }
        }
        return null;
    }

    public ElectricComponent GetNextComponent(ElectricComponent source)
    {
        if (connections.connections.Length != 2)
        {
            throw new Exception("component needs to have only 2 connections");
        }

        foreach (ElectricComponent component in connections.connections)
        {
            if (component != null && component != source)
            {
                return component;
            }
        }

        throw new Exception("component not found");
    }

    public ElectricComponent[] GetAllOtherConnections(ElectricComponent source)
    {
        ElectricComponent[] components = new ElectricComponent[4];
        for (int i = 0; i < connections.connections.Length; i++)
        {
            if (connections.connections[i] != source)
            {
                components[i] = connections.connections[i];
            }
        }
        return components;
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

    public static int GetMultiplierFromConnection(int connection)
    {
        switch (connection)
        {
            case 0: return 0;
            case 2: return 1;
            case 3: return 2;
            case 1: return 3;
            default: throw new System.Exception("Position index must be between 0 and 3 = " + connection);
        } //TODO: revise
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

    public void UpdateConnection()
    {
        DeleteAllConnections();
        AutoConnect();
    }

    #region Visual Connections
    public void UpdateVisualConnections()
    {
        if (HasVisibleConnections)
        {
            for (int i = 0; i < 4; i++)
            {
                if (connections.IsConnected(i)) // si connect� et pas de connection vis.
                {
                    if (visualConnections[i] == null)
                    {
                        CreateVisualConnection(i);
                    }
                } else if (visualConnections[i] != null) // si d�co. et il y a co. vis.
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

        ProjectManager.OnModifyProject(ProjectModificationType.CircuitModification);

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
        if (CanAddConnections())
        {
            connections.SetValue(connectionPosition, component);
            UpdateVisualConnections();
            ProjectManager.OnModifyProject(ProjectModificationType.CircuitModification);
        }
        //else throw new Exception("On Excede le maximum de connection il y a un cas non g�r�. : " + ConnectionCount());
    }
    #endregion

    #region Connection getters
    /*
     * Retourne un array de toutes les connection (Gauche, Droite, Haut, Bas)
     * Sauf celle re�ue en argument
     * Null si il n'y a pas de connection � cet endroit
     */
    public List<ElectricComponent> GetConnectedToExcept(ElectricComponent entryPoint)
    {
        return GetConnectedTo(true, entryPoint);
    }
    /*
     * Retourne un array de toutes les connection(Gauche, Droite, Haut, Bas)
     * Null si il n'y a pas de connection � cet endroit
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
