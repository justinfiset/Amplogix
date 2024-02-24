using SFB;
using System.Collections;
using System.Collections.Generic;
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

    private void Start()
    {
        ProjectSettings projectSettings = (ProjectSettings)FindObjectOfType(typeof(ProjectSettings));
        if (projectSettings != null) 
        {
            project = JsonUtility.FromJson<Project>(projectSettings.data);
        }
        else if(project == null)
        {
            project = new Project();
        }
        UpdateProjectName();

        Debug.Log("Current project: " + project.name);
    }

    public void SetProjectName()
    {
        project.name = nameText.text;
    }

    public void UpdateProjectName()
    {
        nameText.text = project.name;
    }

    public void SaveProject()
    {
        project.name = nameText.text;

        if (project.savePath == "" || project.savePath == null)
        {
            SaveProjectAs();
        } else
        {
            print("Saving project...");
            string data = JsonUtility.ToJson(project);
            FileUtility.WriteString(project.savePath, data);

            isProjectSaved = true;
        }
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
        print("working");
        if(!bypassSaveProtection && !isProjectSaved)
        {
            quitWithoutSavingPopup.Show();
        } else
        {
            SceneManager.LoadScene("MainMenu");
        }
    }
}
