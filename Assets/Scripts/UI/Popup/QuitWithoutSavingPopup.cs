using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
public class QuitWithoutSavingPopup : MonoBehaviour
{
    private CanvasGroup canvasGroup;

    private void Start()
    {
        canvasGroup = GetComponent<CanvasGroup>();
    }

    public void Show()
    {
       canvasGroup.alpha = 1;
    }

    public void Hide()
    {
        canvasGroup.alpha = 0;
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
