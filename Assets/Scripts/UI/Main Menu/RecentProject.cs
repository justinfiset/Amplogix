using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class RecentProject : MonoBehaviour, IPointerExitHandler, IPointerEnterHandler
{
    public string path;
    public TextMeshProUGUI text;
    public GameObject background;
    public GameObject delete;

    public void Start()
    {
        OnExit();
    }

    public void Setup(string name, string path)
    {
        this.path = path;
        text.text = name;
    }

    public void OnClik()
    {
        MainMenuButtons.OpenFile(path);
    }

    public void Delete()
    {
        print("caled");
        MainMenuButtons.RemovePath(path);
        FindObjectOfType<MainMenuButtons>().ResetList();
    }

    public void OnHover()
    {
        background.SetActive(true);
        delete.SetActive(true);
    }

    public void OnExit()
    {
        background.SetActive(false);
        delete.SetActive(false);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (eventData.fullyExited)
        {
            OnExit();
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        OnHover();
    }
}
