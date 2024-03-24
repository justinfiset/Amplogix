using MathNet.Numerics.RootFinding;
using SFB;
using System;
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

    public bool isProjectSaved { get; private set; } = true; // Par d�fault un projet n'a pas de modification

    public Project project;

    [Header("UI")]
    public TMP_InputField nameText;
    public QuitWithoutSavingPopup quitWithoutSavingPopup;
    public GameObject savedIndicator;
    public GameObject isNotSavedIndicator;

    public Dictionary<ElectricComponent, Vector2> componentList { get; private set; }
    public HashSet<ElectricComponent> componentSelection { get; private set; }
    
    void Start()
    {
        if(m_Instance == null) m_Instance = this;
        else Destroy(this);

        componentList = new Dictionary<ElectricComponent, Vector2>();
        componentSelection = new HashSet<ElectricComponent>();
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
            UnityEngine.Object settings = FindObjectOfType(typeof(ProjectSettings));
            if(settings != null)
            {
                Destroy(settings);
            }
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

    public static bool IsSelectionEmpty()
    {
        return m_Instance.componentSelection.Count == 0;
    }

    public static void AddComponentToSelection(ElectricComponent component)
    {
        m_Instance.componentSelection.Add(component);
    }

    public static void RemoveComponentFromSelection(ElectricComponent component)
    {
        m_Instance.componentSelection.Remove(component);
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

    public void ConnectComponents(ElectricComponent first, ElectricComponent second)
    {
        if (ComponentsPointToEachOther(first, second))
        {
            int positionIndex;
            if (AreComponentsAlignedOnPlane(first, second, true)) //si les composants sont al. horizontalement
            {
                float diff = first.transform.position.x - second.transform.position.x;
                // positionIndex = (int) (0.5 + Mathf.Sign(diff) / 2);
                positionIndex = GetPositionIndex(first, second, true);
                first.connectionManager.ConnectTo(positionIndex, second);
                second.connectionManager.ConnectTo(GetOtherValue(positionIndex, 0, 1), first);
            } else
            {
                float diff = first.transform.position.y - second.transform.position.y;
                // positionIndex = (int)(2.5 - Mathf.Sign(diff) / 2);
                positionIndex = GetPositionIndex(first, second, false);
                first.connectionManager.ConnectTo(positionIndex, second);
                second.connectionManager.ConnectTo(GetOtherValue(positionIndex, 2, 3), first);
            }
        }
    }

    public int GetPositionIndex(ElectricComponent first, ElectricComponent second, bool areHorizontal)
    {
        float diff;
        if (areHorizontal)
        {
            diff = first.transform.position.x - second.transform.position.x;
        }
        else
        {
            diff = first.transform.position.y - second.transform.position.y;
        }

        if (areHorizontal)
        {
            if (diff < 0)
            {
                return 1;
            } else return 0;
        } else
        {
            if (diff < 0)
            {
                return 3;
            }
            else return 2;
        }
    }

    private int GetOtherValue(int entry, int first, int second)
    {
        if (entry == first)
        {
            return second;
        }
        if (entry == second)
        {
            return first;
        }
        throw new Exception(entry + " is not value " + first + " or " + second);
    }

    public bool ComponentsPointToEachOther(ElectricComponent first, ElectricComponent second)
    {
        bool firstRespectsOrientation = first.GetComponent<ElectricComponent>().respectOrientation;
        bool secondRespectsOrientation = second.GetComponent<ElectricComponent>().respectOrientation;

        bool isFirstHorizontal = IsComponentHorizontal(first);
        bool isSecondHorizontal = IsComponentHorizontal(second);

        if (firstRespectsOrientation && secondRespectsOrientation)
        {

            if ((isFirstHorizontal == isSecondHorizontal) && AreComponentsAlignedOnPlane(first, second, isFirstHorizontal))
            {
                return true;
            } 
            else
            {
                return false;
            }
        }

        if (firstRespectsOrientation)
        {
            return AreComponentsAlignedOnPlane(first, second, isFirstHorizontal);
        }

        if (secondRespectsOrientation)
        {
            return AreComponentsAlignedOnPlane(first, second, isSecondHorizontal);
        }

        return true;
    }
    #region Alignment checks
    public bool AreComponentsAlignedOnPlane(ElectricComponent first, ElectricComponent second, bool horizontal)
    {
        if (horizontal)
        {
            return AreComponentsHorizontallyAligned(first, second);
        }
        else
        {
            return AreComponentsVerticallyAligned(first, second);
        }
    }

    public bool AreComponentsVerticallyAligned(ElectricComponent first, ElectricComponent second)
    {
        return first.transform.localPosition.x ==  second.transform.localPosition.x;
    }

    public bool AreComponentsHorizontallyAligned(ElectricComponent first, ElectricComponent second)
    {
        return first.transform.localPosition.y == second.transform.localPosition.y;
    }
    #endregion

    #region Orientation checks
    public bool IsComponentHorizontal(ElectricComponent component)
    {
        return Mathf.Abs(component.transform.localEulerAngles.z) % 180 == 0;
    }

    public bool IsComponentVertical(ElectricComponent component)
    {
        return !IsComponentHorizontal(component);
    }
    #endregion

    public void ChangeComponentPos(ElectricComponent component, Vector2 newPos)
    {
        if(componentList.ContainsKey(component))
        {
            componentList[component] = newPos;
        }
    }

    public static void UnselectComponent()
    {
        ElectricComponent[] electricComponentTable = m_Instance.componentSelection.ToArray();
        foreach (ElectricComponent component in electricComponentTable)
        {
            component._Unselect();
        }
    }

    public static void ChangeSelectionColor(Color newColor)
    {
        ElectricComponent[] selection = m_Instance.componentSelection.ToArray();
        foreach(ElectricComponent component in selection)
        {
            component._SetColor(newColor);
        }
    }

    public static void DeleteSelectedComponents()
    {
        ElectricComponent[] selection = m_Instance.componentSelection.ToArray();
        foreach (ElectricComponent component in selection)
        {
            ComponentSpawner.DestroyComponent(component.gameObject);
        }
    }
}
