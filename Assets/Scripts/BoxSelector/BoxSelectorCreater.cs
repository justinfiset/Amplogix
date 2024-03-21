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
    private int layerMask = 5;

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

   // private void FixedUpdate()
    //{
      //  Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        //ray.direction = Vector3.up;
   
  //      hit = Physics2D.Raycast(ray.origin, ray.direction, Mathf.Infinity, 7);
    //    if (hit.collider != null)
      //  {
        //    Debug.DrawLine(ray.origin, hit.point, Color.blue);
          //  Debug.Log("test1");
      //  }
    //}
  

    private bool NoneIsSelected()
    {

        Vector2 min2 =  new Vector2(Input.mousePosition.x, Input.mousePosition.x);
        Vector2 max2 = new Vector2(Input.mousePosition.y + 130, Input.mousePosition.y + 130); ;
        min2 = Camera.main.ScreenToWorldPoint(min2);
        max2 = Camera.main.ScreenToWorldPoint(max2);

        bounds = new Bounds();
        bounds.SetMinMax(min2, max2);

        if (bounds.Contains(new Vector2(75,-75)))
        {
            print("test1");
            return true;
        }
     

        return false;
    }
}
