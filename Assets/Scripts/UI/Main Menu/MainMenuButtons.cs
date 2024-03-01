using SFB;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;
using System.Linq;

public class MainMenuButtons : MonoBehaviour
{
    private static string recentProjectsPrefName = "recentProjects";
    public GameObject list;
    public GameObject recentProjectPrefab;

    private void Start()
    {
        FindRecentProject();
    }
    public void CreateProject()
    {
        SceneManager.LoadScene("CircuitCreator", LoadSceneMode.Single);
    }

    public void LoadModel(/*Model model*/)
    {
        // TODO: LoadModel
    }

    public static void OpenFile()
    {    
        string[] paths = StandaloneFileBrowser.OpenFilePanel("Ouvrir un projet", "", "amp", false);
        OpenFile(paths[0]);    
    }
    public static void OpenFile(string path)
    {
        string data = FileUtility.ReadString(path);

        GameObject gm = Instantiate(new GameObject());
        ProjectSettings settings = gm.AddComponent<ProjectSettings>();
        DontDestroyOnLoad(gm);
        settings.data = data;
        settings.path = path;

        AddRecentProject(path);
        SceneManager.LoadScene("CircuitCreator", LoadSceneMode.Single);
    }

    public static void AddRecentProject(string path)
    {
        string paths = PlayerPrefs.GetString(recentProjectsPrefName);
        if (!paths.Contains(path))
        {
           paths += path + ";"; 
        }      
        PlayerPrefs.SetString(recentProjectsPrefName, paths);
    }

    public void FindRecentProject()
    {
        string paths = PlayerPrefs.GetString(recentProjectsPrefName);
        string[] pathsList = paths.Split(';');
        foreach (string path in pathsList)
        {
           if(path != "")
            {
            string name = Path.GetFileName(path);
            GameObject go = Instantiate(recentProjectPrefab);
            go.transform.SetParent(list.transform);
            go.GetComponent<RecentProject>().Setup(name, path);
            }                      
        }
        
    }

}
