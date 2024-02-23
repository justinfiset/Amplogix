using UnityEngine;

public class ComponentSpawner : MonoBehaviour
{
    public static ComponentSpawner m_Instance { get; private set; }

    public bool canSpawn = true;

    private float spawnAngle;
    private GameObject currentComponent;
    private ComponentSelection currentSelection;

    private Transform parent;

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

    public static void SetCurrentSelection(GameObject prefab, ComponentSelection selection)
    {
        Destroy(m_Instance.componentPreview);

        if (prefab != null)
        {
            m_Instance.currentComponent = prefab;
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
        if(m_Instance.currentComponent != null)
        {
            Vector3 pos = GridSettings.GetCurrentSnapedPosition();
            CreateComponent(m_Instance.currentComponent, pos);
        }
    }

    public static void CreateComponent(GameObject component, Vector3 pos)
    {
        Instantiate(m_Instance.currentComponent, pos, Quaternion.Euler(0, 0, m_Instance.spawnAngle), m_Instance.parent);
        // TODO add component to a list somewhere to keep track
    }

    public static void DestroyComponent(GameObject component)
    {
        Destroy(component);
        // TODO remove from a certain list
    }
}
