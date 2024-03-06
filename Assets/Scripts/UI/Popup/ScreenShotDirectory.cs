using SFB;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ScreenShotDirectory : MonoBehaviour
{
    public string path;
    public TextMeshPro FileName;
    public TextMeshPro Cheminement;

   public void SelectDirectory()
    {
        path = StandaloneFileBrowser.SaveFilePanel("Sauvegarder le projet", "", "Circuit", "png");
    }
}
