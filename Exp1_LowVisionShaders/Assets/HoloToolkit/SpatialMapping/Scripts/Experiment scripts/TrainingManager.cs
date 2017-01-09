using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Windows.Speech;
using HoloToolkit.Unity;
using System;
using UnityEngine.VR.WSA.Input;

// TODO:
// - create Meshes

/// <summary>
/// Experiment Manager: sets experiment states in Update(); Experiment states are switched
/// by clicks (for Development) or taps (either hand or clicker). 
/// </summary>

public class TrainingManager : MonoBehaviour
{
    public TextToSpeechManager textToSpeechManager;     // In editor, drag Speech GameObject
    public GameObject mainCamera;                       // In editor, drag Camera GameObject
    public GameObject spatialMapping;                   // In editor, drag Spatial Mapping GameObject
    public Material DepthMapMaterial;                   // In editor, drag DepthMap Material
    public enum ExperimentState { Idle, WaitToStart, PrepPractice, BeginPractice, Practice, EndPractice, QuitApp };
    public enum ShaderState { Off, Color, Pulse, Transparency, AdjustHoloLens };
    public bool ClickSwitch;
    public ExperimentState currentState = ExperimentState.Idle;
    public ShaderState currentShader = ShaderState.Off;
    public AudioSource audioSource;                     // In editor, drag SpeechFrame AudioSource

    private ClickerManager clickerManager; // reference to ClickerManager (handles taps)
    private float startTime;
    private LogPosRot logPosRot;
    private SpatialMappingManager mappingMaterial;
    private ObjectSurfaceObserver objectSurfaceObserver;


    void Awake()
    {
        // on awake initialize clicker, spatial mapping material and other things
        clickerManager = GetComponent<ClickerManager>();
        ClickSwitch = false;
        logPosRot = mainCamera.GetComponent<LogPosRot>();
        mappingMaterial = spatialMapping.GetComponent<SpatialMappingManager>();
        objectSurfaceObserver = spatialMapping.GetComponent<ObjectSurfaceObserver>();
        var meshmanager = GetComponent<MeshManager>();

        // App always starts off
        mappingMaterial.DrawVisualMeshes = false;
    }


    ///  <summary>
    ///  Switch to a designated state and handle the associated changes to the experimental and stimulus settings
    /// </summary>
    /// <param name = "newState"> The state to switch to</param>
    private void ChangeState(ExperimentState newState)
    {
        // define what happens in each state
        switch (newState)
        {
            case ExperimentState.Idle:
                break;
            case ExperimentState.PrepPractice:
                // Instructions
                startTime = Time.time;                      // Define Starttime for state
                textToSpeechManager.SpeakText("Welcome to our experiment! Let's begin with a quick tutorial. Try walking around the room with the HoloLens and try to get an idea of what is in the room. Click to start.");
                break;
            case ExperimentState.BeginPractice:
                //startTime = Time.time;                      // Define Starttime for state
                textToSpeechManager.SpeakText("Begin!");
                DisplayShader(); // load shader
                break;
            case ExperimentState.Practice:
                startTime = Time.time;                      // Define Starttime for state
                break;
            case ExperimentState.EndPractice:
                mappingMaterial.DrawVisualMeshes = false;   // switch shader off again
                textToSpeechManager.SpeakText("Thank you. Take off the HoloLens and return it to the experimenter.");
                break;
            case ExperimentState.QuitApp:
                Application.Quit(); // Quit App
                break;
            default:
                break;

        }
        currentState = newState;
    }

    void Update()
    {
        // check for user input only if no instructions are being played back
        if (!IsSpeaking())
        {
            ProcessUserInput();
        }
        else if (IsSpeaking())
        {
            Debug.Log("No user input. Audio - is speaking right now: " + IsSpeaking());
        }
        // loop through experiment states and change state if time dependent event
        switch (currentState)
        {
            case ExperimentState.Idle:
                break;
            case ExperimentState.BeginPractice:
                if (Time.time - startTime >= 1) // Go to trial
                {
                    ChangeState(ExperimentState.Practice);
                }
                break;
            case ExperimentState.EndPractice:
                if (Time.time - startTime >= 10) // Go to next phase after a TWO MINUTES
                {
                    ChangeState(ExperimentState.QuitApp);
                }
                break;
            default:
                break;
        }
    }

    // define how and when states are switched on click or tab change experiment state
    // possible options are 1) UserInput (tap or click) 2) on Timer 3) tapCount
    private void ProcessUserInput()
    {
        if (Input.GetButtonDown("Fire1") || ClickSwitch == true)
        {
            switch (currentState)
            {
                case ExperimentState.Idle:
                    ChangeState(ExperimentState.WaitToStart);  // Change Experiment State to next 
                    ClickSwitch = false;                        // Reset the Switch so that clicker works again
                    break;
                case ExperimentState.WaitToStart:
                    ChangeState(ExperimentState.PrepPractice);  // Change Experiment State to next 
                    ClickSwitch = false;                        // Reset the Switch so that clicker works again
                    break;
                case ExperimentState.PrepPractice:
                    ChangeState(ExperimentState.BeginPractice);
                    ClickSwitch = false;
                    break;
                case ExperimentState.Practice:
                    ChangeState(ExperimentState.EndPractice);
                    /*if (Time.time - startTime >= 10) // Go to next phase after a few seconds; Change values to 30 once App is ready
                    {
                        
                    }*/
                    ClickSwitch = false;
                    break;
                default:
                    break;
            }
            // return time and current state for debugging
            Debug.Log("Time: " + Time.time + " current state: " + currentState);
        }

    }

    // switch between shader types
    /// <param name = "newShader"> The state to switch to</param>
    private void ChangeShader(ShaderState newShader)
    {
        switch (newShader)
        {
            // translate depth into
            case ShaderState.Color: // ...color
                DepthMapMaterial.SetInt("_Equation", 6);
                mappingMaterial.DrawVisualMeshes = true;
                break;
            case ShaderState.Transparency: // ... transparency/alpha values
                DepthMapMaterial.SetInt("_Equation", 9);
                mappingMaterial.DrawVisualMeshes = true;
                break;
            case ShaderState.Pulse: // ... frequency
                DepthMapMaterial.SetInt("_Equation", 10);
                mappingMaterial.DrawVisualMeshes = true;
                break;
            case ShaderState.AdjustHoloLens: // white shader to adjust the HoloLens
                DepthMapMaterial.SetInt("_Equation", 12);
                mappingMaterial.DrawVisualMeshes = true;
                break;
            case ShaderState.Off: // white shader to adjust the HoloLens
                mappingMaterial.DrawVisualMeshes = false;
                break;
            default:
                break;
        }
        currentShader = newShader;
        Debug.Log("The new shader STATE is: " + newShader);
    }

    // Displays the shaders according to experimental condition
    private void DisplayShader()
    {
        if (logPosRot.shader == "no_shader")
        {
            ChangeShader(ShaderState.Off);
        }
        else if (logPosRot.shader == "shader_01")
        {
            ChangeShader(ShaderState.Color);
        }
        else if (logPosRot.shader == "shader_02")
        {
            ChangeShader(ShaderState.Transparency);
        }
        else if (logPosRot.shader == "shader_03")
        {
            ChangeShader(ShaderState.Pulse);
        }
        else if (logPosRot.shader == "shader_04")
        {
            ChangeShader(ShaderState.AdjustHoloLens);

        }
        // draw the meshes for the shader
        mappingMaterial.SurfaceMaterial = DepthMapMaterial;  // set mapping material
        mappingMaterial.DrawVisualMeshes = true;             // draw meshes ON
    }

    // test if instructions are being played back
    public bool IsSpeaking()
    {
        if (audioSource != null)
        {
            return audioSource.isPlaying;
        }
        return false;
    }
    // Changes the Mesh for the Object Surface Observer on the Spatial Mapping model 
    // to the model defined in the Experiment GameObject
    public void ChangeMesh()
    {
        var meshmanager = GetComponent<MeshManager>();
        if (logPosRot.layout == "layout_01")
        {
            objectSurfaceObserver.roomModel = meshmanager.Layout1; //SWITCH THIS FEATURE ON ONCE MESHES ARE AVAILABLE
        }
        else if (logPosRot.layout == "layout_02")
        {
            objectSurfaceObserver.roomModel = meshmanager.Layout2;

        }
        else if (logPosRot.layout == "layout_03")
        {
            objectSurfaceObserver.roomModel = meshmanager.Layout3;
        }
        else if (logPosRot.layout == "layout_04")
        {
            objectSurfaceObserver.roomModel = meshmanager.Layout4;

        }

        // switch on the shaders after the correct layout is loaded
        mappingMaterial.Start();
    }
}