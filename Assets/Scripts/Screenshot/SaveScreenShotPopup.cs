using SFB;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;

public class SaveScreenShotPopup : MonoBehaviour
{
    public string path;
    public ScreenShotDirectory manager;
    public TextMeshProUGUI fileName;
    public TextMeshProUGUI cheminement;
    private GameObject container;
   

    private void Start()
    {
        container = transform.GetChild(0).gameObject;
    }

    public void Show()
    {
        container.SetActive(true);
        print("test");
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
        print(path);
        print(Path.GetFileName(path));
    }

    public void Save()
    {
        FindObjectOfType<ScreenShotDirectory>().Save(path);
    }

   
}
