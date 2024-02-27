using SFB;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ProjectManager : MonoBehaviour
{
    public bool isProjectSaved = true; // Par défault un projet n'a pas de modification

    public Project project;

    [Header("UI")]
    public TMP_InputField nameText;
    public QuitWithoutSavingPopup quitWithoutSavingPopup;

    public List<ElectricComponent> componentList;

    public void Init()
    {
        ProjectSettings projectSettings = (ProjectSettings)FindObjectOfType(typeof(ProjectSettings));
        if (projectSettings != null) 
        {
            LoadProject(projectSettings);
        }

        if(project == null)
        {
            project = new Project();
        }
        UpdateProjectName();

        Debug.Log("Current project: " + project.name);
    }

    public void LoadProject(ProjectSettings settings)
    {
        project = settings.GetProject();
        project.savePath = settings.path;

        // Creation des composants
        foreach(ElectricComponentData data in project.componentDataList)
        {
            if(data != null)
            {
                ComponentSpawner.CreateComponent(data);
            }
        }

        isProjectSaved = true;
    }

    public void SetProjectName()
    {
        if(project.name != nameText.text)
        {
            project.name = nameText.text;
            isProjectSaved = false;
        }
    }

    public void UpdateProjectName()
    {
        nameText.text = project.name;
    }

    public void SaveProject()
    {
        project.name = nameText.text; // Just for security

        if (project.savePath == "" || project.savePath == null)
        {
            SaveProjectAs();
        } else
        {
            print("Saving project...");
            SerializeComponents();
            string data = JsonUtility.ToJson(project, true);
            FileUtility.WriteString(project.savePath, data);

            isProjectSaved = true;
        }
    }

    public void SerializeComponents()
    {
        List<ElectricComponentData> data = new List<ElectricComponentData>();
        foreach(ElectricComponent component in componentList)
        {
            data.Add(component.GetData());
        }
        project.componentDataList = data;
    }

    public void SaveProjectAs()
    {
        print("Opening file option window...");
        string path = StandaloneFileBrowser.SaveFilePanel("Sauvegarder le projet", "", nameText.text, "amp");
        project.savePath = path;
        SaveProject();
    }

    public void ReturnToMenu(bool bypassSaveProtection = false)
    {
        if(!bypassSaveProtection && !isProjectSaved)
        {
            quitWithoutSavingPopup.Show();
        } else
        {
            SceneManager.LoadScene("MainMenu");
        }
    }

    public void AddComponent(ElectricComponent component)
    {
        componentList.Add(component);
    }

    public void RemoveComponent(ElectricComponent component)
    {
        componentList.Remove(component);
    }
}
