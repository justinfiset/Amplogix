using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuitWithoutSavingPopup : MonoBehaviour
{
    private GameObject container;

    private void Start()
    {
        container = transform.GetChild(0).gameObject;
    }

    public void Show()
    {
        container.SetActive(true);
    }

    public void Hide()
    {
        container.SetActive(false);
    }

    public void SaveAndExit()
    {
        FindObjectOfType<ProjectManager>().SaveProject();
        Exit();
    }

    public void Exit()
    {
        FindObjectOfType<ProjectManager>().ReturnToMenu(true);
    }
}
