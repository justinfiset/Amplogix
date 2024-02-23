using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraBehaviour : MonoBehaviour
{
    private static CameraBehaviour instance;

    public MonoBehaviour[] controllers;

    public void Start()
    {
        if (instance == null)
        {
            instance = this;
        }
        else Destroy(this);
    }

    public static void DisableControllers()
    {
        foreach (var controller in instance.controllers)
        {
            controller.enabled = false;
        }
    }

    public static void EnableControllers()
    {
        foreach (var controller in instance.controllers)
        {
            controller.enabled = true;
        }
    }
}
