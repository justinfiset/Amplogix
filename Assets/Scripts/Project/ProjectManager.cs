using SFB;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ProjectManager : MonoBehaviour
{
    public static ProjectManager m_Instance { get; private set; }

    public bool isProjectSaved = true; // Par défault un projet n'a pas de modification

    public Project project;

    [Header("UI")]
    public TMP_InputField nameText;
    public QuitWithoutSavingPopup quitWithoutSavingPopup;

    public Dictionary<UnityEngine.Vector2, ElectricComponent> componentList;

    void Start()
    {
        if(m_Instance == null) m_Instance = this;
        else Destroy(this);

        componentList = new Dictionary<UnityEngine.Vector2, ElectricComponent>();
    }

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
        foreach (ElectricComponent component in componentList.Values)
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
        componentList.Add(component.transform.position, component);
    }

    public void RemoveComponent(ElectricComponent component)
    {
        componentList.Remove(component.transform.position);
    }

    public bool ContainsComponent(UnityEngine.Vector2 position)
    {
        bool containsKey = false;

        if (componentList.Count() > 1)
        {
            containsKey = componentList.ContainsKey(position);
        }
        return containsKey;
    }

    public ElectricComponent GetComponent(Vector2 position)
    {
        ElectricComponent component = null;

        if(ContainsComponent(position))
        {
            component = componentList[position];
        }

        return component;
    }

    public List<KeyValuePair<Vector2, ElectricComponent>> GetSurroundingComponents(Vector2 pos)
    {
        List<KeyValuePair<Vector2, ElectricComponent>> list = new List<KeyValuePair<Vector2, ElectricComponent>>();
        
        float posDiff = GridSettings.m_Instance.gridIncrement;
        Vector2 xDiff = new Vector2(posDiff, 0);
        Vector2 yDiff = new Vector2(0, posDiff);

        List<Vector2> positions = new List<Vector2>
        {
            pos - xDiff, // Gauche
            pos + xDiff, // Droite
            pos + yDiff, // Haut
            pos - yDiff, // Bas
        };

        foreach(Vector2 position in positions)
        {
            ElectricComponent component;
            component = GetComponent(position);
            if(component != null)
            {
                list.Add(new KeyValuePair<Vector2, ElectricComponent>(position, component));
            }
        }

        return list;
    }

    public List<Vector2> GetSurroundingComponentsPos(Vector2 pos)
    {
        List<Vector2> list = new List<Vector2>();

        float posDiff = GridSettings.m_Instance.gridIncrement;
        Vector2 xDiff = new Vector2(posDiff, 0);
        Vector2 yDiff = new Vector2(0, posDiff);

        List<Vector2> positions = new List<Vector2>
        {
            pos - xDiff, // Gauche
            pos + xDiff, // Droite
            pos + yDiff, // Haut
            pos - yDiff, // Bas
        };

        foreach (Vector2 position in positions)
        {
            if (ContainsComponent(position))
            {
                list.Add(position);
            }
        }

        return list;
    }

}
