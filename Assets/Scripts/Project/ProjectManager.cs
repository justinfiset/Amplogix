using SFB;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ProjectManager : MonoBehaviour
{
    public static ProjectManager m_Instance { get; private set; }

    public bool isProjectSaved { get; private set; } = true; // Par d�fault un projet n'a pas de modification

    public Project project;

    public static int componentUnderPointerCount = 0;

    [Header("UI")]
    public TMP_InputField nameText;
    public QuitWithoutSavingPopup quitWithoutSavingPopup;
    public GameObject savedIndicator;
    public GameObject isNotSavedIndicator;

    public Dictionary<ElectricComponent, Vector2> componentList { get; private set; }
    public int selectionCount = 0;
    public HashSet<ElectricComponent> componentSelection { get; private set; }
    public static bool canInteract = true;

    public bool isGUIinit = false;

    public static int GetSelectionCount()
    {
        return m_Instance.selectionCount;
    }

    void Start()
    {
        if(m_Instance == null) m_Instance = this;
        else Destroy(this);

        componentList = new Dictionary<ElectricComponent, Vector2>();
        componentSelection = new HashSet<ElectricComponent>();
    }

    public void OnGUI()
    {
        if(!isGUIinit)
        {
            if(ComponentGUI.InitGUI())
            {
                print($"<color=#00FFFF>GUI initialised !</color>");
                isGUIinit = true;
            }
        }
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
                component.SetBaseResistance(data.resistance);
                component.initialConnectionData = data.connectionData;
                component.initialComponentData = data.customComponentData;
            }
        }
        OnSaveProject();

        StartCoroutine(InitializeAllConnections());
    }

    IEnumerator InitializeAllConnections()
    {
        yield return new WaitForNextFrameUnit();
        foreach (ElectricComponent component in componentList.Keys)
        {
            component.InitConnections();
        }
    }

    public void SetProjectName()
    {
        if(project.name != nameText.text)
        {
            project.name = nameText.text;
            OnModifyProject(ProjectModificationType.ProjectDataModification);
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
            SerializeComponents();
            string data = JsonUtility.ToJson(project, true);
            FileUtility.WriteString(project.savePath, data);

            OnSaveProject();
            MainMenuButtons.AddRecentProject(project.savePath);
            print($"<color=#00FF00>Project saved...</color>");
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
        StandaloneFileBrowser.SaveFilePanelAsync("Sauvegarder le projet", Application.dataPath, nameText.text, "amp", delegate (string path)
        {
            project.savePath = path;
            SaveProject();
            print($"<color=#00FF00>Project saved...</color>");
        });
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

    public static void OnModifyProject(ProjectModificationType type)
    {
        m_Instance.isProjectSaved = false;
        m_Instance.savedIndicator.SetActive(false);
        m_Instance.isNotSavedIndicator.SetActive(true);

        if (type == ProjectModificationType.CircuitModification || type == ProjectModificationType.CircuitDataModification)
        {

            SimulationManager.ProjectModificationCallback(); // TODO appeler dans un fonction moin appelé
            CurrentVisualisationManager.ResetParticleEmissions();
        }
    }

    public static bool IsSelectionEmpty()
    {
        return m_Instance.selectionCount == 0;
    }

    public static void AddComponentToSelection(ElectricComponent component)
    {
        if(m_Instance.componentSelection.Add(component))
        {
            m_Instance.selectionCount++;
        }
    }

    public static void RemoveComponentFromSelection(ElectricComponent component)
    {
        if(m_Instance.componentSelection.Remove(component))
        {
            m_Instance.selectionCount--;
        }
    }

    public void AddComponent(ElectricComponent component)
    {
        componentList.Add(component, component.transform.position);
        OnModifyProject(ProjectModificationType.CircuitModification);
    }

    public void RemoveComponent(ElectricComponent component)
    {
        componentList.Remove(component);
        OnModifyProject(ProjectModificationType.CircuitModification);
    }

    public bool ContainsComponent(Vector2 pos)
    {
        return componentList.ContainsValue(pos);
    }

    public bool ContainsComponent(ElectricComponent component)
    {
        return componentList.ContainsKey(component);
    }

    public static ElectricComponent GetComponent(Vector2 position)
    {
        foreach(KeyValuePair<ElectricComponent, Vector2> pair in m_Instance.componentList)
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

    public List<KeyValuePair<Vector2, ElectricComponent>> GetSurroundingComponentsWithNulls(Vector2 pos)
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

        foreach (Vector2 position in positions)
        {
            ElectricComponent component;
            component = GetComponent(position);
            
            list.Add(new KeyValuePair<Vector2, ElectricComponent>(position, component));
        }

        return list;
    }

    public void ConnectComponents(ElectricComponent first, ElectricComponent second)
    {
        //print("CONNECTING " + first + " AND " + second);
        if(first.connectionManager.CanAddConnections() && second.connectionManager.CanAddConnections())
        {
            if (ComponentsPointToEachOther(first, second))
            {
                int positionIndex;
                if (AreComponentsAlignedOnPlane(first, second, true)) //si les composants sont al. horizontalement
                {
                    positionIndex = GetPositionIndex(first, second, true);
                    first.connectionManager.ConnectTo(positionIndex);
                    second.connectionManager.ConnectTo(GetOtherValue(positionIndex, 0, 1));
                }
                else
                {
                    positionIndex = GetPositionIndex(first, second, false);
                    first.connectionManager.ConnectTo(GetOtherValue(positionIndex, 2, 3));
                    second.connectionManager.ConnectTo(positionIndex);
                }
            }
        }
    }

    public int GetPositionIndex(ElectricComponent first, ElectricComponent second)
    {
        return GetPositionIndex(first, second, AreComponentsAlignedOnPlane(first, second, true));
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

    public static void ResetCurrentIntensity()
    {
        ElectricComponent[] selection = m_Instance.componentList.Keys.ToArray();
        foreach (ElectricComponent component in selection)
        {
            component.SetCalculatedIntensity(0f);
        }
    }

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

    public static void ChangeAllComponentsColor(Color newColor)
    {
        ElectricComponent[] selection = m_Instance.componentList.Keys.ToArray();
        foreach (ElectricComponent component in selection)
        {
            component._SetColor(newColor);
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
            component._DestroyComponent();
        }
    }

    public static List<ElectricComponent> GetAllConnectedComponents()
    {
        List<ElectricComponent> connectedComponents = new List<ElectricComponent> ();

        foreach(ElectricComponent component in m_Instance.componentList.Keys)
        {
            if(component.canGenerateMeshes && component.connectionManager.ConnectionCount() > 1) // Si il possède plus de deux connection / nécessaire pour avoir une maille circulaire
            {
                connectedComponents.Add(component);
            }
        }

        return connectedComponents;
    }

    public static List<ElectricComponent> GetAllElectricComponentsOfType(ElectricComponentType type)
    {
        return GetAllElectricComponentsOfType(m_Instance.componentList.Keys.ToList(), type);
    }

    public static List<ElectricComponent> GetAllElectricComponentsOfType(List<ElectricComponent> components, ElectricComponentType type)
    {
        List<ElectricComponent> componentsOfType = new List<ElectricComponent> ();
        foreach(ElectricComponent component in components)
            if(component.type == type)
                componentsOfType.Add(component);
        return componentsOfType;
    }

    public static bool ComponentCountIsValid(List<ElectricComponent> list)
    {
        // On veut assez de composants et au moin une source
        bool countIsValid = list.Count > 0;

        if(countIsValid)
        {
            foreach (ElectricComponent comp in list)
            {
                if (comp.type == ElectricComponentType.PowerSource)
                {
                    return true;
                }
            }
        }

        return false;
    }
}
