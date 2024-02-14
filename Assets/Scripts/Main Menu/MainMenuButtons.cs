using SFB;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuButtons : MonoBehaviour
{
    public void CreateProject()
    {
        SceneManager.LoadScene("CircuitCreator", LoadSceneMode.Single);
    }
   

    public void LoadModel(/*Model model*/)
    {
        // TODO: LoadModel
    }

    public void OpenFile()
    {    
        var paths = StandaloneFileBrowser.OpenFilePanel("Ouvrir fichier", "", "amp", false);

    }
}
