using SFB;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class ScreenshotManager : MonoBehaviour
{
    private Camera cam;

    private void Start()
    {
        cam = GetComponent<Camera>();
    }

    public string SelectDirectory()
    {
        return StandaloneFileBrowser.SaveFilePanel("Enregistrer le schéma", "", "Circuit", "png");
    }

    public void SaveCurrentView(string path)
    {
        SavecamView(cam, path);
    }

    public Texture2D GetCurrentViewTexture()
    {
        return GetCurrentViewTexture(cam);
    }

    public static Texture2D GetCurrentViewTexture(Camera cam)
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

    public static void SavecamView(Camera cam, string path)
    {
        Texture2D renderedTexture = GetCurrentViewTexture(cam);
        byte[] byteArray = renderedTexture.EncodeToPNG();
        File.WriteAllBytes(path, byteArray);
    }
}
