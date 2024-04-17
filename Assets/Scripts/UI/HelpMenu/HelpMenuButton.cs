using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HelpMenuButton : MonoBehaviour
{
    public void ReturnButton()
    {
        SceneManager.LoadScene("MainMenu", LoadSceneMode.Single);
    }
}
