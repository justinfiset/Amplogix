using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using static FourWayConnection;

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


    public class Connections
    {
        public bool left = false;
        public bool right = false;
        public bool top = false;
        public bool bottom = false;
    }
}
