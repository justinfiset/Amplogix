using UnityEngine;

public class CameraDrag : MonoBehaviour
{
    private Vector3 resetPos;
    private Vector3 originPos;
    private Vector3 diffPos;
    private bool isDragging = false;

    public KeyCode dragButton = KeyCode.Mouse1;

    private Camera cam;

    void Start()
    {
        cam = GetComponent<Camera>();
        resetPos = cam.transform.position;
    }

    void LateUpdate()
    {
        if (Input.GetKey(dragButton))
        {
            diffPos = (cam.ScreenToWorldPoint(Input.mousePosition)) - cam.transform.position;
            if (isDragging == false)
            {
                isDragging = true;
                originPos = cam.ScreenToWorldPoint(Input.mousePosition);
            }
        }
        else
        {
            isDragging = false;
        }
        if (isDragging == true)
        {
            cam.transform.position = originPos - diffPos;
        }
        
        if (Input.GetKeyDown(KeyCode.Space))
        {
            ResetCamPos();
        }
    }

    public void ResetCamPos()
    {
        cam.transform.position = resetPos;
    }
}