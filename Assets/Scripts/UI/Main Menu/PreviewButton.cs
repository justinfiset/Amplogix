using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PreviewButton : MonoBehaviour
{
    public string modelData;
    public void LoadModel()
    {
        GameObject gm = Instantiate(new GameObject());
        ProjectSettings settings = gm.AddComponent<ProjectSettings>();
        DontDestroyOnLoad(gm);
        settings.data = modelData;
        SceneManager.LoadScene("CircuitCreator", LoadSceneMode.Single);
    }
}
