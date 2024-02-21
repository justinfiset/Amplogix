using SFB;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ProjectManager : MonoBehaviour
{
    public bool isProjectSaved = false;

    public TMP_InputField nameText;

    

    public Project project;

    private void Start()
    {
        nameText.interactable = true;
        nameText.text = "Nom du projet";
        ProjectSettings projectSettings = (ProjectSettings)FindObjectOfType(typeof(ProjectSettings));

        if (projectSettings != null) 
        {
            project = JsonUtility.FromJson<Project>(projectSettings.data);
        }

        else 
        { 
            project = new Project();
            UpdateProjectName();
        }
        Debug.Log(project.name);
    }

    public void UpdateProjectName()
    {
        nameText.text = project.name;
    }

    public void OpenNewProject()
    {
        // TODO: Open New Project
    }

    public void SaveProject()
    {
        if (project.savePath == null)
        {
            SaveProjectAs();
        }
        // TODO: Save the project

        string data = JsonUtility.ToJson(project);
       FileUtility.WriteString(project.savePath, data);
       

        isProjectSaved = true;
    }

    public void SaveProjectAs()
    {
        if (project.savePath == null)
        {
            string path = StandaloneFileBrowser.SaveFilePanel("Sauvegarder le projet", "", nameText.text, "amp");
            project.savePath = path;
        }
        SaveProject();
    }

    public void ReturnToMenu()
    {
        if(!isProjectSaved)
        {
            // TODO: Want to save project popup???
            SceneManager.LoadScene("MainMenu");
        }
    }
}
