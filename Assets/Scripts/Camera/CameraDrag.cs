using UnityEngine;

public class CameraDrag : MonoBehaviour
{
    private Vector3 ResetCamera;
    private Vector3 Origin;
    private Vector3 Diference;
    private bool Drag = false;

    public bool canDrag = true;

    public KeyCode dragButton = KeyCode.Mouse1;

    void Start()
    {
        ResetCamera = Camera.main.transform.position;
    }
    void LateUpdate()
    {
        //canDrag = CursorManager.Instance.canDrag();
        if (!canDrag) return;

        if (Input.GetMouseButton(0))
        {
            Diference = (Camera.main.ScreenToWorldPoint(Input.mousePosition)) - Camera.main.transform.position;
            if (Drag == false)
            {
                Drag = true;
                Origin = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            }
        }
        else
        {
            Drag = false;
        }
        if (Drag == true)
        {
            Camera.main.transform.position = Origin - Diference;
        }

        if(Input.GetKeyDown(dragButton))
        {
            CursorManager.ChangeCursor(CursorManager.WindowsCursor.Hand);
        }
        if(Input.GetKeyUp(dragButton))
        {
            CursorManager.ChangeCursor(CursorManager.WindowsCursor.StandardArrow);
        }

        //CursorManager.Instance.isDragin = Drag;

        //RESET CAMERA TO STARTING POSITION WHEN SPACEBAR DOWN
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Camera.main.transform.position = ResetCamera;
        }
    }
}