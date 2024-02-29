using UnityEngine;

public class CameraDrag : MonoBehaviour
{
    private Vector3 resetPos;
    private Vector3 originPos;
    private Vector3 diffPos;
    private bool isDragging = false;

    public KeyCode dragButton = KeyCode.Mouse1;

    void Start()
    {
        resetPos = Camera.main.transform.position;
    }

    void LateUpdate()
    {
        if (Input.GetKey(dragButton))
        {
            diffPos = (Camera.main.ScreenToWorldPoint(Input.mousePosition)) - Camera.main.transform.position;
            if (isDragging == false)
            {
                isDragging = true;
                originPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            }
        }
        else
        {
            isDragging = false;
        }
        if (isDragging == true)
        {
            Camera.main.transform.position = originPos - diffPos;
        }
        
        if (Input.GetKeyDown(KeyCode.Space))
        {
            ResetCamPos();
        }
    }

    public void ResetCamPos()
    {
        Camera.main.transform.position = resetPos;
    }
}