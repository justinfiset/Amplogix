using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class Project : MonoBehaviour
{
    public string name;
    public string savePath;
    
    // Une map qui contient tout les circuits
    Dictionary<string, Circuit> circuits = new Dictionary<string, Circuit>();
}
