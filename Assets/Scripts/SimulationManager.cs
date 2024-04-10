using MathNet.Numerics.LinearAlgebra;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class SimulationManager : MonoBehaviour
{
    public static SimulationManager Instance { get; private set; }

    public GameObject playButton;
    public GameObject pauseButton;
    public GameObject stopButton;
    public TextMeshProUGUI timeText;

    // STATE
    public bool timeShouldBeHandled = false;
    public float startTime; // temps en seconde
    public bool isSimulating = false;
    public bool isPaused = false;

    private void Start()
    {
        if (Instance == null)
            Instance = this;
        else Destroy(this);
    }

    public static void ProjectModificationCallback()
    {
        Instance.OnModifyProject();
    }

    public void OnModifyProject()
    {
        Stop();
        StartCoroutine(HandleCircuitOnNextFrame());
    }

    
    public IEnumerator HandleCircuitOnNextFrame()
    {
        yield return new WaitForFrames(1);
        HandleCircuit();
    }

    public static void HandleCircuit()
    {
        MatrixEquationSystem circuitData = MeshBuilder.CreateAndCalculateMeshes();
    }

    private void Update()
    {
        // TODO GESTION DU TEMPS EN PAUSE
        if (isSimulating)
        {
            if(!isPaused)
            {
                timeText.text = (Time.time - startTime).ToString();
            }
        }
        else
        {
            timeText.text = "";
        }
    }

    public void UpdateButtons()
    {
        if (isSimulating)
        {
            stopButton.SetActive(true);
            if (isPaused)
            {
                playButton.SetActive(true);
                pauseButton.SetActive(false);
            } else
            {
                playButton.SetActive(false);
                pauseButton.SetActive(true);
            }
        }
        else
        {
            stopButton.SetActive(false);
            playButton.SetActive(true);
            pauseButton.SetActive(false);
        }
    }

    public void Play()
    {
        isSimulating = true;
        isPaused = false;

        if (isPaused)
        {

        }
        else
        {
            startTime = Time.time;
            HandleCircuit();
        }

        UpdateButtons();
    }

    public void Pause()
    {
        isPaused = true;

        UpdateButtons();
    }

    public void Stop()
    {
        isSimulating = false;
        isPaused = false;

        UpdateButtons();
    }
}
