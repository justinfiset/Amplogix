using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

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
    }

    public void Hide()
    {
        container.SetActive(false);
    }

    public void SelectDirectory()
    {
        path = manager.SelectDirectory();
        fileName.text = Path.GetFileName(path);
        cheminement.text = path;
        saveButton.interactable = true;
    }

    public void Save()
    {
        FindObjectOfType<ScreenshotManager>().SaveCurrentView(path);
    }

   
}
