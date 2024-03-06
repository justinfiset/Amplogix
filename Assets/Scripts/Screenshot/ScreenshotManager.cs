using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenshotManager : MonoBehaviour
{
    Camera camera;

    public void Start()
    {
        camera = GetComponent<Camera>();
    }
    public void FindCamera()
    {
        SaveCameraView(camera);
    }

    public void SaveCameraView(Camera cam)
    {
        RenderTexture screenTexture = new RenderTexture(Screen.width, Screen.height, 16);
        cam.targetTexture = screenTexture;
        RenderTexture.active = screenTexture;
        cam.Render();

        Texture2D renderedTexture = new Texture2D(Screen.width, Screen.height);
        renderedTexture.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
        RenderTexture.active = null;

        byte[] byteArray = renderedTexture.EncodeToPNG();
        System.IO.File.WriteAllBytes(Application.dataPath + "/cameracapture.png", byteArray);
    }
}
