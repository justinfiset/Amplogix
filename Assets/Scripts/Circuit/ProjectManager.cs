using SFB;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ProjectManager : MonoBehaviour
{
    public bool isProjectSaved = false;

    [SerializeField] private TextMeshProUGUI nameText;

    public Project project;

    private void Start()
    {
        if(project == null)
        {
            project = new Project();
            UpdateProjectName();
        }
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
