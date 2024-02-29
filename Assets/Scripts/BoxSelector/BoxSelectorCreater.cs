using Unity.VisualScripting;
using UnityEngine;

public class BoxSelectorCreater : MonoBehaviour
{
    private RectTransform rectangle;
    public GameObject prefabs;
    public Transform parent;
    private Vector2 initialMousePos;
    private bool firstTime = true;

    private Quaternion rotation;
    private Vector2 mousePos;
    private Vector2 scaleChange;

    void Start()
    {
        
    }

    void Update()
    {
        if (Input.GetMouseButton(0))
        {
           if (firstTime) {
                initialMousePos = Input.mousePosition;
                
                firstTime = false;
                rectangle = Instantiate(prefabs, initialMousePos, rotation, parent).GetComponent<RectTransform>();
            }

            scaleChange = Input.mousePosition - (Vector3) initialMousePos * 2;

            rectangle.sizeDelta = new Vector2 (Mathf.Abs(scaleChange.x), Mathf.Abs(scaleChange.y));
            rectangle.anchoredPosition = initialMousePos + scaleChange;
        }
        if (Input.GetMouseButtonUp(0)) {
            Destroy(rectangle.gameObject);
            firstTime = true;
        }
    }
}
