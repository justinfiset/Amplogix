using System;
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
    float witdh;
    float height;

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
                rectangle.gameObject.SetActive(true);
            }

            witdh = Input.mousePosition.x - initialMousePos.x;
            height = Input.mousePosition.y - initialMousePos.y;

            rectangle.anchoredPosition = (initialMousePos) + new Vector2 (witdh/2,height/2);
            rectangle.sizeDelta = new Vector2(Math.Abs(witdh), Math.Abs(height));
        }

        if (Input.GetMouseButtonUp(0)) {
            Destroy(rectangle.gameObject);
            firstTime = true;
        }
    }
}
