using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class DeleteComponent : MonoBehaviour
{
    private bool mouseIsOverObject;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    if (Input.GetMouseButtonDown(2) && mouseIsOverObject)
        {
            Destroy(gameObject);
        }
    }

    private void OnMouseEnter()
    {
        mouseIsOverObject = true;
    }

    private void OnMouseExit()
    {
        mouseIsOverObject = false;
    }
}
