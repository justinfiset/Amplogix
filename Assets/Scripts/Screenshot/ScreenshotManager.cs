using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenshotManager : MonoBehaviour
{
    private Camera camera;

    public void Start()
    {
        camera = GetComponent<Camera>();
    }

    public void SaveAttachedCamera()
    {
        saveScreenShotPopup.Show();
    }
}
