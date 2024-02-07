using UnityEngine;

public class CameraScroll : MonoBehaviour
{
    private Camera cam;

    public float cursorFollowStep = 1f;

    public float sensibility = 10f;
    public float minSize;
    public float maxSize;
    public bool canScroll = true;

    private void Start()
    {
        cam = GetComponent<Camera>();
    }

    void Update()
    {
        float axis = Input.GetAxis("Mouse ScrollWheel");
        if (canScroll && axis != 0f)
        {
            // Adjust zoom
            cam.orthographicSize += sensibility * axis * -1f;

            if (cam.orthographicSize < minSize)
            {
                cam.orthographicSize = minSize;
                return;
            }
            else if (cam.orthographicSize > maxSize)
            {
                cam.orthographicSize = maxSize;
                return;
            }
            
            Vector3 cursorPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3 deltaPos = axis >= 0 ? (cam.transform.position - cursorPos) : Vector3.zero;
            Vector3 translation = deltaPos * cursorFollowStep * axis * -1f;
            cam.transform.position = cam.transform.position + translation;
        }
    }
}
