using System.Collections;
using System.Collections.Generic;
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
        cam = this.GetComponent<Camera>();
    }
    void Update()
    {
        if (!canScroll) return;

        float axis = Input.GetAxis("Mouse ScrollWheel");
        if (axis != 0f)
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
            Vector3 deltaPos = (cam.transform.position - cursorPos) * axis * -1f;
            Vector3 translation = deltaPos * cursorFollowStep;
            cam.transform.position = cam.transform.position + translation;
        }
    }
}
