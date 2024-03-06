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
           rectangle.sizeDelta = Vector2.zero;
            //scaleChange = Input.mousePosition - (Vector3) initialMousePos;
  
            witdh = Input.mousePosition.x - initialMousePos.x;
            height = Input.mousePosition.y - initialMousePos.y;

            print(rectangle.anchoredPosition + "avant" + rectangle.position);

            rectangle.anchoredPosition = (initialMousePos) + new Vector2 (witdh/2,height/2);

            print(rectangle.anchoredPosition + "apres" + rectangle.position);

            rectangle.sizeDelta = new Vector2(Math.Abs(witdh), Math.Abs(height));

          
        }
        if (Input.GetMouseButtonUp(0)) {
            Destroy(rectangle.gameObject);
            firstTime = true;
        }
    }
}
