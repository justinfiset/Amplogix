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
        string[] paths = StandaloneFileBrowser.OpenFilePanel("Ouvrir un projet", "", "amp", false);
        string data = FileUtility.ReadString(paths[0]);

        GameObject gm = Instantiate(new GameObject());
        ProjectSettings settings = gm.AddComponent<ProjectSettings>();
        DontDestroyOnLoad(gm);
        settings.data = data;

        SceneManager.LoadScene("CircuitCreator", LoadSceneMode.Single);
    }
}
