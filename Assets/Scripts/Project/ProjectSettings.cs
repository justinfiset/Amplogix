using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectSettings : MonoBehaviour
{
    public string data;

    public Project GetProject()
    {
        Project project = null;

        project = JsonUtility.FromJson<Project>(data);

        return project;
    }
}
