using UnityEngine;

public class ComponentSpawner : MonoBehaviour
{
    public static ComponentSpawner m_Instance { get; private set; }

    [HideInInspector] private GameObject currentComponent;
    public Transform parent;
    public bool canSpawn = true;

    private ComponentSelection currentSelection;

    void Start()
    {
        if (m_Instance == null)
        {
            m_Instance = this;
        }
        else Destroy(this);
    }

    void Update()
    {
        if (canSpawn && Input.GetMouseButtonDown(0))
        {
            SpawnPrefab();
        }
    }

    public static void SetCurrentSelection(GameObject prefab, ComponentSelection selection)
    {
        if (prefab != null)
        {
            m_Instance.currentComponent = prefab;
            m_Instance.canSpawn = true;
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
        if(m_Instance.currentComponent != null)
        {
            Vector3 pos = GridSettings.GetCurrentSnapedPosition();
            CreateComponent(m_Instance.currentComponent, pos);
        }
    }

    public static void CreateComponent(GameObject component, Vector3 pos)
    {
        Instantiate(m_Instance.currentComponent, pos, Quaternion.identity, m_Instance.parent);
        // TODO add component to a list somewhere to keep track
    }

    public static void DestroyComponent(GameObject component)
    {
        Destroy(component);
        // TODO remove from a certain list
    }
}
