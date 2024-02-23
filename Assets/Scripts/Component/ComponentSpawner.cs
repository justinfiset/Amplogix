using UnityEngine;

public class ComponentSpawner : MonoBehaviour
{
    public static ComponentSpawner m_Instance { get; private set; }

    [HideInInspector] private GameObject currentComponent;
    public Transform parent;
    public bool canSpawn = true;

    private ComponentSelection currentSelection;

    public GameObject previewPrefab;
    private GameObject componentPreview;

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
        Destroy(m_Instance.componentPreview);

        if (prefab != null)
        {
            m_Instance.currentComponent = prefab;
            m_Instance.canSpawn = true;

            m_Instance.componentPreview = Instantiate(m_Instance.previewPrefab, m_Instance.parent);
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
