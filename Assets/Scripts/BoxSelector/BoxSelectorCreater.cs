using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

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


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

       
        if (Input.GetMouseButton(0))
        {
            print("debut");
           if (firstTime) {
                initialMousePos = Input.mousePosition;
                
                firstTime = false;
                rectangle = Instantiate(prefabs, initialMousePos, rotation, parent)
                    .GetComponent<RectTransform>();
                
            }
            print("mouseInit" + initialMousePos);
            print("rect" + rectangle.position);
            print("mouse" + Input.mousePosition);
            scaleChange = Input.mousePosition - (Vector3) initialMousePos;

            rectangle.sizeDelta = scaleChange.Abs()*2;
            rectangle.anchoredPosition = initialMousePos + scaleChange;
            print("fin");
        }
        if (Input.GetMouseButtonUp(0)) {
            Destroy(rectangle.gameObject);
            firstTime = true;
        }
    }
}
