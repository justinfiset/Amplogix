using SFB;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ProjectManager : MonoBehaviour
{
    public TextMeshProUGUI nameText;
    
  
    // Start is called before the first frame update
    void Start()
    {

    }

    public void SaveProject()
    {
        var paths = StandaloneFileBrowser.SaveFilePanel("Sauvegarder fichier", "", nameText.text, "amp");
    }

    // Update is called once per frame
    void Update()
    {
    }
   
}
