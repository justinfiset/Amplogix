using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Project : MonoBehaviour
{
    string name;
    string savePath;
    
    // Une map qui contient tout les circuits
    Dictionary<string, Circuit> circuits = new Dictionary<string, Circuit>();

    public void Save()
    {
        // TODO : Save
    }

    public void SaveAs(string path = null)
    {
        if(path == null)
        {
            // TODO : POPUP
            // path = nouveau chemin
        }
        savePath = path;
        Save();
    }
}
