using SFB;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;

public class ScreenShotDirectory : MonoBehaviour
{
    private Camera camera;

    private void Start()
    {
        camera = GetComponent<Camera>();
    }

    public string SelectDirectory()
    {
        string path = StandaloneFileBrowser.SaveFilePanel("Sauvegarder le projet", "", "Circuit", "png");
        return path;
    }

    public void Save(string path)
    {
        SaveCameraView(camera, path);
    }

    public static void SaveCameraView(Camera cam, string path)
    {
        RenderTexture screenTexture = new RenderTexture(Screen.width, Screen.height, 16);
        cam.targetTexture = screenTexture;
        RenderTexture.active = screenTexture;
        cam.Render();

        Texture2D renderedTexture = new Texture2D(Screen.width, Screen.height);
        renderedTexture.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
        RenderTexture.active = null;

        byte[] byteArray = renderedTexture.EncodeToPNG();
        System.IO.File.WriteAllBytes(path, byteArray);
    }
}
