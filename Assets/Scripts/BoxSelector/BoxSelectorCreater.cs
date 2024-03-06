using System;
using System.Collections.Generic;
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
    private float witdh;
    private float height;
    private Bounds bounds;
    



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
  
            witdh = Input.mousePosition.x - initialMousePos.x;
            height = Input.mousePosition.y - initialMousePos.y;

      

            rectangle.anchoredPosition = (initialMousePos) + new Vector2 (witdh/2,height/2);
            rectangle.sizeDelta = new Vector2(Math.Abs(witdh), Math.Abs(height));
        }




        if (Input.GetMouseButtonUp(0)) {
            
            firstTime = true;

            Vector2 min = rectangle.anchoredPosition - (rectangle.sizeDelta / 2);
            Vector2 max = rectangle.anchoredPosition + (rectangle.sizeDelta / 2);
            min = Camera.main.ScreenToWorldPoint(min);
            max = Camera.main.ScreenToWorldPoint(max);

            bounds = new Bounds();
            bounds.SetMinMax(min, max);

            foreach (KeyValuePair<ElectricComponent, Vector2> kvp in ProjectManager.m_Instance.componentList)
            {
                if (bounds.Contains(kvp.Value))
                {
                    kvp.Key._Select();
                }
            }
            Destroy(rectangle.gameObject);
        }
    }

    
}
