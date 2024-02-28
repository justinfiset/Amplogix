using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(TextMeshProUGUI))]
public class RecentProject : MonoBehaviour
{
    public string path;
    public TextMeshProUGUI text;

    public void Setup(string name, string path)
    {
        this.path = path;
        text.text = name;
    }

    public void OnClik()
    {
        MainMenuButtons.OpenFile(path);
    }
    
}
