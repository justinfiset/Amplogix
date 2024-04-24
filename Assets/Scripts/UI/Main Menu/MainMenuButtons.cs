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

    public static void OpenFile()
    {
        StandaloneFileBrowser.OpenFilePanelAsync("Ouvrir un projet", "", "amp", false, delegate (string[] paths)
        {
            if(paths.Length > 0)
            {
                string path = paths[0];
                if (File.Exists(path))
                {
                    OpenFile(paths[0]);
                }
            }
        });    
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
        List<string> pathsList = new List<string>(paths.Split(';'));
        List<string> newList = new List<string>();
        bool hasChanged = false; // On nettoie la liste en même temps
        foreach (string path in pathsList)
        {
            if(path != null && path != "")
            {
                if(File.Exists(path))
                {
                    string name = Path.GetFileName(path);
                    GameObject go = Instantiate(recentProjectPrefab);
                    go.transform.SetParent(list.transform);
                    go.GetComponent<RecentProject>().Setup(name, path);
                    newList.Add(path);
                }
                else
                {
                    hasChanged = true;
                }
            } 
        }

        if(hasChanged)
        {
            string newPathsString = string.Join(";", newList);
            PlayerPrefs.SetString(recentProjectsPrefName, newPathsString);
        }
    }

    public static void RemovePath(string path)
    {
        string paths = PlayerPrefs.GetString(recentProjectsPrefName);
        List<string> pathsList = new List<string>(paths.Split(';'));
        pathsList.Remove(path);

        string newPathsString = string.Join(";", pathsList);
        PlayerPrefs.SetString(recentProjectsPrefName, newPathsString);

    }

    public void ResetList()
    {
        for(int i = 0; i < list.transform.childCount; i++)
        {
            Destroy(list.transform.GetChild(i).gameObject);
        }

        FindRecentProject();
    }

    public void OpenHelpScene()
    {
        SceneManager.LoadScene("HelpMenu", LoadSceneMode.Single);
    }
}
