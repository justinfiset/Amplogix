using Unity.VisualScripting;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

[Serializable]
public class ElectricComponentPrefabLinker
{
    public ElectricComponentType type;
    public GameObject prefab;
}

public class ComponentSpawner : MonoBehaviour
{
    public static ComponentSpawner m_Instance { get; private set; }
    public ProjectManager projectManager;

    public bool canSpawn = true;

    private float spawnAngle;
    private ElectricComponentType currentComponentType;
    public List<ElectricComponentPrefabLinker> prefabList;
    private ComponentSelection currentSelection;

    [SerializeField] private Transform parent;

    public GameObject previewPrefab;
    private GameObject componentPreview;

    [Header("Inputs")]
    public static KeyCode rotateKey = KeyCode.R;

    void Start()
    {
        if (m_Instance == null)
        {
            m_Instance = this;
        }
        else Destroy(this);

        projectManager.Init();
    }

    void Update()
    {
        if (canSpawn && Input.GetMouseButtonDown(0))
        {
            SpawnPrefab();
        }

        if (Input.GetKeyDown(rotateKey))
        {
            componentPreview.transform.Rotate(Vector3.forward * -90);
            spawnAngle = componentPreview.transform.eulerAngles.z;
        }
    }

    private GameObject GetCurrentPrefab()
    {
        foreach(ElectricComponentPrefabLinker linker in prefabList)
        {
            if(linker.type == currentComponentType)
            {
                return linker.prefab;
            }
        }
        return null;
    }

    private GameObject GetPrefab(ElectricComponentType type)
    {
        foreach (ElectricComponentPrefabLinker linker in prefabList)
        {
            if (linker.type == type)
            {
                return linker.prefab;
            }
        }
        return null;
    }

    public static void SetCurrentSelection(ElectricComponentType type, ComponentSelection selection)
    {
        Destroy(m_Instance.componentPreview);

        if (type != ElectricComponentType.None)
        {
            m_Instance.currentComponentType = type;
            m_Instance.canSpawn = true;

            m_Instance.componentPreview = Instantiate(m_Instance.previewPrefab, m_Instance.parent);
            m_Instance.spawnAngle = m_Instance.componentPreview.transform.eulerAngles.z;

            SpriteRenderer sprite = m_Instance.componentPreview.GetComponent<SpriteRenderer>();
            sprite.sprite = selection.sprite;
            sprite.color = new Color(0, 0, 0, 0.25f);
        }
        else
        {
            m_Instance.canSpawn = false;
        }

        if(m_Instance.currentSelection != null)
        {
            m_Instance.currentSelection.OnUnselect();
        }
        m_Instance.currentSelection = selection;
        m_Instance.currentSelection.OnSelect();
    }

    private static void SpawnPrefab()
    {
        GameObject currentComponent = m_Instance.GetCurrentPrefab();
        if (currentComponent != null)
        {
            Vector3 pos = GridSettings.GetCurrentSnapedPosition();
            CreateComponent(currentComponent, pos);
        }
    }

    public static void CreateComponent(GameObject component, Vector3 pos)
    {
        CreateComponent(component, pos, Quaternion.Euler(0, 0, m_Instance.spawnAngle), Vector3.one);
    }

    public static void CreateComponent(ElectricComponentData data)
    {
        ElectricComponentType type = (ElectricComponentType)data.type;
        GameObject component = m_Instance.GetPrefab(type);
        Vector3 pos = new Vector3(data.x, data.y, 0);
        Quaternion angles = Quaternion.Euler(0, 0, data.rot);
        Vector3 scale = new Vector3(data.scaleX, data.scaleY, 1);
        CreateComponent(component, pos, angles, scale); 
    }

    public static void CreateComponent(GameObject component, Vector3 pos, Quaternion spawnAngle, Vector3 scale)
    {
        GameObject instance = Instantiate(component, pos, spawnAngle, m_Instance.parent);
        m_Instance.projectManager.isProjectSaved = false;
        m_Instance.projectManager.AddComponent(instance.GetComponent<ElectricComponent>());
        instance.transform.localScale = scale;
    }

    public static void DestroyComponent(GameObject component)
    {
        m_Instance.projectManager.AddComponent(component.GetComponent<ElectricComponent>());
        Destroy(component);
        m_Instance.projectManager.isProjectSaved = false;
    }
}
