using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;

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
    public RaycastHit2D hit;
    public RaycastHit hit3d;

    private bool nothingSelected;
   

    void Start()
    {
  
    }

    void Update()
    {
        if (Input.GetMouseButton(0))
        {   
           nothingSelected = ProjectManager.IsSelectionEmpty();

           if (firstTime && IsNotInComponentArea() && nothingSelected) {
                initialMousePos = Input.mousePosition;
                
                firstTime = false;
                rectangle = Instantiate(prefabs, initialMousePos, rotation, parent).GetComponent<RectTransform>();
                rectangle.gameObject.SetActive(true);
            }

            if (rectangle == null) return;

            rectangle.sizeDelta = Vector2.zero;
  
            witdh = Input.mousePosition.x - initialMousePos.x;
            height = Input.mousePosition.y - initialMousePos.y;

            rectangle.anchoredPosition = (initialMousePos) + new Vector2 (witdh/2,height/2);
            rectangle.sizeDelta = new Vector2(Math.Abs(witdh), Math.Abs(height));
        }

        if (Input.GetMouseButtonUp(0)) 
        {
            firstTime = true;

            if (rectangle == null) return;

            Vector2 min = rectangle.anchoredPosition - (rectangle.sizeDelta / 2);
            Vector2 max = rectangle.anchoredPosition + (rectangle.sizeDelta / 2);
            min = Camera.main.ScreenToWorldPoint(min);
            max = Camera.main.ScreenToWorldPoint(max);

            bounds = new Bounds();
            bounds.SetMinMax(min, max);

            HashSet<ElectricComponent> components = new HashSet<ElectricComponent>();
            foreach (KeyValuePair<ElectricComponent, Vector2> kvp in ProjectManager.m_Instance.componentList)
            {
                if (bounds.Contains(kvp.Value))
                {
                    components.Add(kvp.Key);
                }
            }


            bool isUnique = components.Count == 1 && ProjectManager.IsSelectionEmpty();
            foreach (ElectricComponent component in components)
            {
                component._Select(isUnique);
            }

            Destroy(rectangle.gameObject);
        }
    }


    private bool IsNotInComponentArea(){
        if(Input.mousePosition.x <= 150 ){
            return false;
        }
        return true;
    }
}
