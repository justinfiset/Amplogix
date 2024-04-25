 using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using SFB;

public class SaveScreenShotPopup : MonoBehaviour
{
    public string path;

    public ScreenshotManager manager;
    public TextMeshProUGUI fileName;
    public TextMeshProUGUI cheminement;
    private GameObject container;
    public Button saveButton;

    private void Start()
    {
        container = transform.GetChild(0).gameObject;
    }

    public void Show()
    {
        container.SetActive(true);
        saveButton.interactable = false;
        fileName.text = "";
        cheminement.text = "";
        ProjectManager.UnselectComponent();
        ProjectManager.canInteract = false;
    }

    public void Hide()
    {
        ProjectManager.canInteract = true;
        container.SetActive(false);
    }

    public void SelectDirectory()
    {
        StandaloneFileBrowser.SaveFilePanelAsync("Enregistrer le schéma", Application.dataPath, "Circuit", "png", delegate (string path)
        {
            if(Path.IsPathRooted(path))
            {
                fileName.text = Path.GetFileNameWithoutExtension(path);
                cheminement.text = path;
                saveButton.interactable = true;
            }
        });
    }

    public void Save()
    {
        ProjectManager.UnselectComponent();
        manager.SaveCurrentView(path);        
        Hide();
        ProjectManager.canInteract = true;
    }
}
