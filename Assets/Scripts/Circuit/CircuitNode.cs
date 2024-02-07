using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircuitNode : MonoBehaviour
{
    // Related Electric Component
    public ElectricComponent component { get; private set; }

    // Nodes
    public HashSet<CircuitNode> previousNodes { get; private set; }
    public HashSet<CircuitNode> nextNodes { get; private set; }
    
    public void AddFollowingNode(CircuitNode newNode)
    {
        nextNodes.Add(newNode);
    }
}
