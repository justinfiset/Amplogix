using SFB;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ProjectManager : MonoBehaviour
{
    public static ProjectManager m_Instance { get; private set; }

    public bool isProjectSaved { get; private set; } = true; // Par défault un projet n'a pas de modification

    public Project project;

    [Header("UI")]
    public TMP_InputField nameText;
    public QuitWithoutSavingPopup quitWithoutSavingPopup;
    public GameObject savedIndicator;
    public GameObject isNotSavedIndicator;

    public Dictionary<ElectricComponent, Vector2> componentList;

    void Start()
    {
        if(m_Instance == null) m_Instance = this;
        else Destroy(this);

        componentList = new Dictionary<ElectricComponent, Vector2>();
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
        OnSaveProject();
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
                ElectricComponent component = ComponentSpawner.CreateComponent(data);
                component.initialComponentData = data.customComponentData;
            }
        }

        OnSaveProject();
    }

    public void SetProjectName()
    {
        if(project.name != nameText.text)
        {
            project.name = nameText.text;
            OnModifyProject();
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

            OnSaveProject();
            MainMenuButtons.AddRecentProject(project.savePath);
        }
    }

    public void SerializeComponents()
    {
        List<ElectricComponentData> data = new List<ElectricComponentData>();
        foreach (ElectricComponent component in componentList.Keys)
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

    public static void OnSaveProject()
    {
        m_Instance.isProjectSaved = true;
        m_Instance.savedIndicator.SetActive(true);
        m_Instance.isNotSavedIndicator.SetActive(false);
    }

    public static void OnModifyProject()
    {
        m_Instance.isProjectSaved = false;
        m_Instance.savedIndicator.SetActive(false);
        m_Instance.isNotSavedIndicator.SetActive(true);
    }

    public void AddComponent(ElectricComponent component)
    {
        componentList.Add(component, component.transform.position);
        OnModifyProject();
    }

    public void RemoveComponent(ElectricComponent component)
    {
        componentList.Remove(component);
        OnModifyProject();
    }

    public bool ContainsComponent(Vector2 pos)
    {
        return componentList.ContainsValue(pos);
    }

    public bool ContainsComponent(ElectricComponent component)
    {
        return componentList.ContainsKey(component);
    }

    public ElectricComponent GetComponent(Vector2 position)
    {
        foreach(KeyValuePair<ElectricComponent, Vector2> pair in componentList)
        {
            if(pair.Value.Equals(position))
            {
                return pair.Key;
            }
        }
        return null;
    }

    public int GetComponentCount(Vector2 pos)
    {
        int count = 0;
        foreach(Vector2 compPos in componentList.Values)
        {
            if (compPos == pos)
            {
                count++;
            }
        }
        return count;
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

    public void ChangeComponentPos(ElectricComponent component, Vector2 newPos)
    {
        if(componentList.ContainsKey(component))
        {
            componentList[component] = newPos;
        }
    }
}
