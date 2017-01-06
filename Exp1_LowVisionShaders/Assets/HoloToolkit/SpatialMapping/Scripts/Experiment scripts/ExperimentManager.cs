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

public class ExperimentManager : MonoBehaviour
{
    public TextToSpeechManager textToSpeechManager;     // In editor, drag Speech GameObject
    public GameObject mainCamera;                       // In editor, drag Camera GameObject
    public GameObject spatialMapping;                   // In editor, drag Spatial Mapping GameObject
    public Material DepthMapMaterial;                   // In editor, drag DepthMap Material
    public enum     ExperimentState { Idle, WaitToStart, PrepTasks, InstrTask1, BeginTask1, Task1, EndTask1, BeginTask2, Task2, EndTask2, QuitApp };
    public enum     ShaderState {Off, Color, Pulse, Transparency, AdjustHoloLens};
    public bool     ClickSwitch;
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
        clickerManager        = GetComponent<ClickerManager>();
        ClickSwitch           = false;
        logPosRot             = mainCamera.GetComponent<LogPosRot>();
        mappingMaterial       = spatialMapping.GetComponent<SpatialMappingManager>();
        objectSurfaceObserver = spatialMapping.GetComponent<ObjectSurfaceObserver>();
        var meshmanager       = GetComponent<MeshManager>();
        
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
            case ExperimentState.PrepTasks:
                startTime = Time.time;                      // Define Starttime for state
                textToSpeechManager.SpeakText("Once you are in the room and ready to start, click to go to the next step.");
                break;
            case ExperimentState.InstrTask1:
                startTime = Time.time;                      // Define Starttime for state
                //textToSpeechManager.SpeakText("Your first task is to find a chair and sit down as quickly as possible. Use the clicker as soon as you sat down. Click to start when you are ready!");
                textToSpeechManager.SpeakText("Click to start!");
                // ChangeMesh(); // loads an existing room model (high res mesh) and uses that instead of the online mesh. DON'T DO THIS YET.
                break;
            case ExperimentState.BeginTask1:
                startTime = Time.time;                      // Define Starttime for state
                textToSpeechManager.SpeakText("Go!");
                DisplayShader();
                //mappingMaterial.DrawVisualMeshes = true;    // switch shader on
                break;
            case ExperimentState.Task1:
                startTime = Time.time;                      // Define Starttime for state
                break;
            case ExperimentState.EndTask1:
                startTime = Time.time;                      // Define Starttime for state
                mappingMaterial.DrawVisualMeshes = false;   // shader off
                textToSpeechManager.SpeakText("Great! You've completed the first task! Wait for instructions.");
                break;
            case ExperimentState.BeginTask2:
                textToSpeechManager.SpeakText("Go!");
                //mappingMaterial.DrawVisualMeshes = true;
                break;
            case ExperimentState.Task2:
                startTime = Time.time;
                break;
            case ExperimentState.EndTask2:
                startTime = Time.time;
                mappingMaterial.DrawVisualMeshes = false;
                textToSpeechManager.SpeakText("Great! You've completed the second task! Keep the HoloLens and the low vision goggles on while the experimenter guides you out of the room.");
                break;
            case ExperimentState.QuitApp:
                //startTime = Time.time;                      // Define Starttime for state
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
        if (!IsSpeaking()) {
            ProcessUserInput();
        } else if (IsSpeaking())
        {
            Debug.Log("No user input. Audio - is speaking right now: " + IsSpeaking());
        }
        // loop through experiment states and change state if time dependent event
        switch (currentState)
        {
            case ExperimentState.Idle:
                break;             
            case ExperimentState.BeginTask1:
                if (Time.time - startTime >= 1) // Go to trial
                {
                    ChangeState(ExperimentState.Task1);
                }
                break;
            case ExperimentState.BeginTask2:
                if (Time.time - startTime >= 1) // Go to trial
                {
                    ChangeState(ExperimentState.Task2);
                }
                break;
            case ExperimentState.Task2:
                if (Time.time - startTime >= 30) // Go to next phase after a TWO MINUTES
                {
                    ChangeState(ExperimentState.EndTask2);
                }
                break;
            case ExperimentState.EndTask2:
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
                    ChangeState(ExperimentState.PrepTasks);
                    ClickSwitch = false;
                    break;
                case ExperimentState.PrepTasks:
                    ChangeState(ExperimentState.InstrTask1);
                    ClickSwitch = false;
                    break;
                case ExperimentState.InstrTask1:
                    ChangeState(ExperimentState.BeginTask1);
                    ClickSwitch = false;
                    break;
                case ExperimentState.BeginTask1:
                    //ChangeState(ExperimentState.Task1);
                    ClickSwitch = false;
                    break;
                case ExperimentState.Task1:
                    ChangeState(ExperimentState.EndTask1);
                    ClickSwitch = false;
                    break;
                case ExperimentState.EndTask1:
                    ChangeState(ExperimentState.BeginTask2);
                    ClickSwitch = false;
                    break;
                case ExperimentState.BeginTask2:
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