using MathNet.Numerics.LinearAlgebra;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class SimulationManager : MonoBehaviour
{
    public static SimulationManager Instance { get; private set; }

    public int orientationModifier = 1; // Par défaut, on est dans le sens conventionel (horaire)

    public GameObject playButton;
    public GameObject pauseButton;
    public GameObject stopButton;
    public TextMeshProUGUI timeText;

    // DATA
    private MatrixEquationSystem circuitData;

    // STATE
    public bool timeShouldBeHandled = false;
    public float startTime; // temps en seconde
    public bool isSimulating = false;
    public bool isPaused = false;
    public bool calculAreLaunched = false;

    // ORIENTATION
    [SerializeField] private Toggle conventionalOrientationIndicator;
    [SerializeField] private Toggle electronOrientationIndicator;

    public void UpdateOrientationUI()
    {
        if (orientationModifier == 1)
        { // Sens conventionel
            conventionalOrientationIndicator.interactable = false;
            electronOrientationIndicator.interactable = true;
        }
        else
        { // Sens des éléctrons
            conventionalOrientationIndicator.interactable = true;
            electronOrientationIndicator.interactable = false;
        }
    }

    public void SetCurrentOrientation(int newOrientation)
    {
        orientationModifier = newOrientation;
        ProjectManager.OnModifyProject(ProjectModificationType.CircuitDataModification);
        UpdateOrientationUI();
        Play();
    }

    public void OnClickConventionalOrientation()
    {
        SetCurrentOrientation(1);
    }

    public void OnClickElectronOrientation()
    {
        SetCurrentOrientation(-1);
    }

    private void Start()
    {
        if (Instance == null)
            Instance = this;
        else Destroy(this);

        UpdateOrientationUI();
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
        if(!calculAreLaunched) // On lance les calculs une seule fois par frame
        {
            calculAreLaunched = true;
            yield return new WaitForFrames(2);
            HandleCircuit();
        }
    }

    public void HandleCircuit()
    {
        circuitData = MeshBuilder.CreateAndCalculateMeshes(orientationModifier);
        calculAreLaunched = false;
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

        if (isPaused)
        {
            Resume();
            isPaused = false;
        }
        else
        {
            isPaused = false;
            startTime = Time.time;
            HandleCircuit();
            // CODE :
            CurrentVisualisationManager.StartEmission(circuitData, orientationModifier);
        }    

        UpdateButtons();
    }

    public void Resume()
    {
        // CODE :
        CurrentVisualisationManager.ResumeEmission(orientationModifier);
    }

    public void Pause()
    {
        isPaused = true;
        UpdateButtons();
        // CODE :
        CurrentVisualisationManager.PauseEmission();
    }

    public void Stop()
    {
        isSimulating = false;
        isPaused = false;
        UpdateButtons();
        // CODE :
        CurrentVisualisationManager.StopEmission();
    }
}
