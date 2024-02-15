using SFB;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ProjectManager : MonoBehaviour
{
    bool isProjectSaved = false;

    public TextMeshProUGUI nameText;

    public void SaveProject()
    {
        string path = StandaloneFileBrowser.SaveFilePanel("Sauvegarder fichier", "", nameText.text, "amp");
    }

    public void ReturnToMenu()
    {
        if(!isProjectSaved)
        {
            // TODO: Want to save project popup???
            SceneManager.LoadScene("MainMenu");
        }
    }
}
