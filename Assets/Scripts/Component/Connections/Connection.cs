using System.Collections.Generic;
using UnityEngine;

public abstract class Connection : MonoBehaviour
{
    public enum ConnectionPosition { Left, Right, Top, Bottom }
    protected Connections connections;

    // Start is called before the first frame update
    void Start()
    {
        connections = new Connections();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void DeleteAllConnections()
    {
        connections.top = false; connections.bottom = false;
        connections.left = false; connections.right = false;
    }

    public void ConnectTo(ConnectionPosition connectionPosition)
    {
        switch (connectionPosition)
        {
            case ConnectionPosition.Left:connections.left = true; break;
            case ConnectionPosition.Right:connections.right = true; break;
            case ConnectionPosition.Top:connections.top = true; break;
            case ConnectionPosition.Bottom:connections.bottom = true; break;
        }
    }

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
