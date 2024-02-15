using UnityEngine;

public class Dragable : MonoBehaviour
{
    private float startPoxX;
    private float startPoxY;
    private bool isBeingHeld;
    private bool snapToGrid = true;

    // Update is called once per frame
    void Update()
    {
        if (isBeingHeld)
        {
            if(snapToGrid)
            {   // We snap the object according to the grid settings
                gameObject.transform.localPosition = GridSettings.GetCurrentSnapedPosition();
            }
            else
            {
                // We move the object according to the mouse position
                Vector3 mousePos;
                mousePos = Input.mousePosition;
                mousePos = Camera.main.ScreenToWorldPoint(mousePos);

                gameObject.transform.localPosition = new Vector3(mousePos.x - startPoxX, mousePos.y - startPoxY, 0);
            }
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
