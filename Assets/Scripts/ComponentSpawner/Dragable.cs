using System.Collections;
using System.Collections.Generic;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Dragable : MonoBehaviour
{
    private float startPoxX;
    private float startPoxY;
    private bool isBeingHeld;
   

    // Update is called once per frame
    void Update()
    {
        if (isBeingHeld && SceneManager.GetActiveScene().name == "CircuitCreator")
        {
            Vector3 mousePos;
            mousePos = Input.mousePosition;
            mousePos = Camera.main.ScreenToWorldPoint(mousePos);

            this.gameObject.transform.localPosition = new Vector3(mousePos.x - startPoxX, mousePos.y - startPoxY, 0);
        }
    }


    private void OnMouseOver()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (isBeingHeld)
            {
                isBeingHeld = false;
            }
            else
            {
                Vector3 mousePos;
                mousePos = Input.mousePosition;
                mousePos = Camera.main.ScreenToWorldPoint(mousePos);

                startPoxX = mousePos.x - this.transform.localPosition.x;
                startPoxY = mousePos.y - this.transform.localPosition.y;

                isBeingHeld = true;
            }
        }

       
    }

  

}
