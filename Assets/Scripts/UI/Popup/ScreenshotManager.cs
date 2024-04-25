using SFB;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
using UnityEngine;

public class ScreenshotManager : MonoBehaviour
{
    private Camera cam;

    public Camera mainCam;

    private void Start()
    {
        cam = GetComponent<Camera>();
    }

    private void CopyMainCamSize(Camera cam)
    {
        cam.orthographicSize = mainCam.orthographicSize;
    }

    public void SaveCurrentView(string path)
    {
        SavecamView(cam, path);
    }

    private Texture2D GetCurrentViewTexture()
    {
        return GetCurrentViewTexture(cam);
    }

    private static Texture2D GetCurrentViewTexture(Camera cam)
    {
        RenderTexture screenTexture = new RenderTexture(Screen.width, Screen.height, 16);
        cam.targetTexture = screenTexture;
        RenderTexture.active = screenTexture;
        cam.Render();

        Texture2D renderedTexture = new Texture2D(Screen.width, Screen.height);
        renderedTexture.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
        RenderTexture.active = null;
        return renderedTexture;
    }

    public void SavecamView(Camera cam, string path)
    {
        CopyMainCamSize(cam);
        Texture2D renderedTexture = GetCurrentViewTexture(cam);
        byte[] byteArray = renderedTexture.EncodeToPNG();
        File.WriteAllBytes(path, byteArray);
    }
}
