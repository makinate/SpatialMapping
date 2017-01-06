using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Windows.Speech;
using HoloToolkit.Unity;
using System;

public class VoicePlacement : MonoBehaviour
{

    /// <summary>
    /// Tracks if we have been sent a transform for the model.
    /// The model is rendered relative to the actual anchor.
    /// </summary>
    public bool GotTransform { get; private set; }

    /// <summary>
    /// Materials to be displayed - used to by this script to set the value of
    /// the Spatial Mapping Manager's shared material
    /// </summary>
    public Material NormalMaterial;
    public Material DepthMapMaterial;
    public Material Wireframe;



    /// <summary>
    /// Used to get a reference to the Spatial Mapping Manager and shared material
    /// </summary>
    private GameObject Player;
    private MeshRenderer meshRenderer;
    private GameObject MainMenu;
    private GameObject Camera;
    private GameObject Experiment;

    SpatialMappingManager mappingMaterial;
    LogPosRot eventManager;
    LoadOnTab loadOnTab;
    ExperimentLoop trialStageManager;

    /// <summary>
    /// When the experience starts, we disable all of the rendering of the model.
    /// </summary>
    List<MeshRenderer> disabledRenderers = new List<MeshRenderer>();

    /// <summary>
    /// We use a voice command to enable moving the target.
    /// </summary>
    KeywordRecognizer keywordRecognizer;

    /// <summary>
    /// Initializes the voice placement functionality - get a reference to the Spatial
    /// Mapping Manager and set up the keyword recognizer
    /// </summary>
    void Start()
    {
        // Get Spatial Mapping Manager so we can access it's shared material later
        Player = GameObject.Find("SpatialMapping");
        mappingMaterial = Player.GetComponent<SpatialMappingManager>();

        // set current VoiceCommand to "Listening"
        Camera = GameObject.Find("Main Camera");
        eventManager = Camera.GetComponent<LogPosRot>();

        MainMenu = GameObject.Find("MainMenu");

        eventManager.tempEvent = "Listening";
        print(eventManager.tempEvent);

        Experiment = GameObject.Find("Experiment");



        // Setup a keyword recognizer to enable resetting the target location.
        List<string> keywords = new List<string>();

        // Keywords to select between different shaders
        keywords.Add("Set Shader Normal");
        keywords.Add("Set Shader Depth");
        keywords.Add("Set Shader Wireframe");

        keywords.Add("Adjust HoloLens");

        // Keywords to turn the shaders on and off
        keywords.Add("Shader Off");
        keywords.Add("Shader On");

        // Keywords to adjust the depth map shader equation
        keywords.Add("Depth Map Standard");
        keywords.Add("Depth Map Inverted");
        keywords.Add("Depth Map Scaled");
        keywords.Add("Depth Map Non Linear");
        keywords.Add("Depth Map Color 4");
        keywords.Add("Depth Map Color 5");
        keywords.Add("Depth Map Color 6");
        keywords.Add("Depth Map Red to Blue continuous");  // color map from Red to Blue in 6 steps
        keywords.Add("Depth Map Red to Blue six");  // color map from Red to Blue in 6 steps
        keywords.Add("Depth Map Red to Blue five"); // color map from Red to Blue in 5 steps
        keywords.Add("Depth Map Yellow to Blue six");  // Yellow to Blue
        keywords.Add("Depth Map Yellow to Blue five");
        keywords.Add("Depth Map Yellow to Red six");  // Yellow to Red
        keywords.Add("Depth Map Yellow to Red five");
        keywords.Add("Depth Map Flicker");
        keywords.Add("Depth Map Dynamic");
        keywords.Add("Depth Map Discrete");
        keywords.Add("Depth Map Pulse");
        keywords.Add("Test Pulse");

        // Keywords to adjust the depth map shader color
        keywords.Add("Set Color White");
        keywords.Add("Set Color Red");
        keywords.Add("Set Color Green");
        keywords.Add("Set Color Blue");
        keywords.Add("Set Color Yellow");
        keywords.Add("Set Color Orange");
        keywords.Add("Set Color Purple");
        keywords.Add("Set Color Grey");

        // Keywords for experiment cycle
        keywords.Add("Begin Training");
        keywords.Add("Begin Trial");
        keywords.Add("Found Chair");
        keywords.Add("End Trial");

        // Initialize keyword recognizer
        keywordRecognizer = new KeywordRecognizer(keywords.ToArray());
        keywordRecognizer.OnPhraseRecognized += KeywordRecognizer_OnPhraseRecognized;
        keywordRecognizer.Start();

        // App always starts off
        mappingMaterial.DrawVisualMeshes = false;
    }

    /// <summary>
    /// Listens for voice commands and initiates the necessary changes in display and
    /// material - users can choose between three materials and turn the display on and
    /// off by voice
    /// </summary>
    /// <param name="args"></param>
    private void KeywordRecognizer_OnPhraseRecognized(PhraseRecognizedEventArgs args)
    {
        if (args.text.Equals("Set Shader Normal"))
        {
            mappingMaterial.SurfaceMaterial = NormalMaterial;
            mappingMaterial.DrawVisualMeshes = true;
            eventManager.tempEvent = "Shader Normal";

            // Set shader to the Depth Map shader
        }
        else if (args.text.Equals("Set Shader Depth"))
        {
            mappingMaterial.SurfaceMaterial = DepthMapMaterial;
            mappingMaterial.DrawVisualMeshes = true;
            eventManager.tempEvent = "Shader Depth";

            // Set shader to the Wireframe shader
        }
        else if (args.text.Equals("Set Shader Wireframe"))
        {
            mappingMaterial.SurfaceMaterial = Wireframe;
            mappingMaterial.DrawVisualMeshes = true;
            eventManager.tempEvent = "Shader Wireframe";

            // Overlay scene with a bright white shader to adjust HoloLens
        }
        else if (args.text.Equals("Adjust HoloLens"))
        {
            mappingMaterial.SurfaceMaterial = DepthMapMaterial;
            mappingMaterial.DrawVisualMeshes = true;
            DepthMapMaterial.SetInt("_Equation", 18);
            eventManager.tempEvent = "Adjust HoloLens";

            // Turn the shader off
        }
        else if (args.text.Equals("Shader Off"))
        {
            mappingMaterial.DrawVisualMeshes = false;
            eventManager.tempEvent = "Shader Off";

            // Turn the shader on
        }
        else if (args.text.Equals("Shader On"))
        {
            mappingMaterial.DrawVisualMeshes = true;
            eventManager.tempEvent = "Shader On";

            // Tell depth map to use standard equation
        }
        else if (args.text.Equals("Depth Map Standard"))
        {
            DepthMapMaterial.SetInt("_Equation", 0);
            eventManager.tempEvent = "Map Standard";

            // Tell depth map to use inverted equation
        }
        else if (args.text.Equals("Depth Map Inverted"))
        {
            DepthMapMaterial.SetInt("_Equation", 1);
            eventManager.tempEvent = "Depth Map Inverted";

            // Tell depth map to use scaled equation
        }
        else if (args.text.Equals("Depth Map Scaled"))
        {
            DepthMapMaterial.SetInt("_Equation", 2);
            eventManager.tempEvent = "Depth Map Scaled";

            // Tell depth map to use non-linear equation
        }
        else if (args.text.Equals("Depth Map Non Linear"))
        {
            DepthMapMaterial.SetInt("_Equation", 3);
            eventManager.tempEvent = "Depth Map Non Linear";

            // Tell depth map to use color coded equation 1; 4 discrete distance caetgories
        }
        else if (args.text.Equals("Depth Map Color 4"))
        {
            DepthMapMaterial.SetInt("_Equation", 4);
            eventManager.tempEvent = "Depth Map Color Coded";

            // Tell depth map to use color coded equation 2; 5 discrete distance caetgories
        }
        else if (args.text.Equals("Depth Map Color 5"))
        {
            DepthMapMaterial.SetInt("_Equation", 5);
            eventManager.tempEvent = "Depth Map Color 5";

            // Tell depth map to use color coded equation 3; 6 discrete distance caetgories
        }
        else if (args.text.Equals("Depth Map Color 6"))
        {
            DepthMapMaterial.SetInt("_Equation", 6);
            eventManager.tempEvent = "Depth Map Color 6";

        }

        // flickering shader: cycles between two shaders at a constant frequency; discrete distance caetgories; green
        else if (args.text.Equals("Depth Map Flicker"))
        {
            DepthMapMaterial.SetInt("_Equation", 7);
            eventManager.tempEvent = "Depth Map Flicker";

        }
        // dynamically flickering distance shader: flicker rate decreases with distance; discrete distance caetgories; green
        else if (args.text.Equals("Depth Map Dynamic"))
        {
            DepthMapMaterial.SetInt("_Equation", 8);
            eventManager.tempEvent = "Depth Map Dynamic";

        }
        // decreasing alpha values for increasing distance; discrete distance caetgories; green
        else if (args.text.Equals("Depth Map Discrete"))
        {
            DepthMapMaterial.SetInt("_Equation", 9);
            eventManager.tempEvent = "Depth Map Discrete";

        }
        // pulsating green shader with different pulse frequencies for discrete distance categories
        else if (args.text.Equals("Depth Map Pulse"))
        {
            DepthMapMaterial.SetInt("_Equation", 10);
            eventManager.tempEvent = "Depth Map Pulse";

        }
        // Brighter pulsating green shader
        else if (args.text.Equals("Bright Pulse"))
        {
            DepthMapMaterial.SetInt("_Equation", 11);
            eventManager.tempEvent = "Depth Map Bright Pulse";

        }
        else if (args.text.Equals("Depth Map Red to Blue six"))
        {
            DepthMapMaterial.SetInt("_Equation", 12);
            eventManager.tempEvent = "Depth Map Red to Blue six";
        }
        else if (args.text.Equals("Depth Map Red to Blue five"))
        {
            DepthMapMaterial.SetInt("_Equation", 13);
            eventManager.tempEvent = "Depth Map Red to Blue five";
        }
        else if (args.text.Equals("Depth Map Yellow to Blue six"))
        {
            DepthMapMaterial.SetInt("_Equation", 14);
            eventManager.tempEvent = "Depth Map Yellow to Blue six";
        }
        else if (args.text.Equals("Depth Map Yellow to Blue five"))
        {
            DepthMapMaterial.SetInt("_Equation", 15);
            eventManager.tempEvent = "Depth Map Yellow to Red five";
        }
        else if (args.text.Equals("Depth Map Yellow to Red six"))
        {
            DepthMapMaterial.SetInt("_Equation", 16);
            eventManager.tempEvent = "Depth Map Yellow to Red six";
        }
        else if (args.text.Equals("Depth Map Yellow to Red five"))
        {
            DepthMapMaterial.SetInt("_Equation", 17);
            eventManager.tempEvent = "Depth Map Red to Blue five";
        }
        // depth map from red to blue
        else if (args.text.Equals("Depth Map Red to Blue continuous"))
        {
            DepthMapMaterial.SetInt("_Equation", 19);
            eventManager.tempEvent = "Depth Map Red to Blue continuous";
        }
        // Tell depth map to use white
        else if (args.text.Equals("Set Color White"))
        {
            DepthMapMaterial.SetInt("_Color", 0);

            // Tell depth map to use red 
        }
        else if (args.text.Equals("Set Color Red"))
        {
            DepthMapMaterial.SetInt("_Color", 1);

            // Tell depth map to use green
        }
        else if (args.text.Equals("Set Color Green"))
        {
            DepthMapMaterial.SetInt("_Color", 2);

            // Tell depth map to use blue
        }
        else if (args.text.Equals("Set Color Blue"))
        {
            DepthMapMaterial.SetInt("_Color", 3);

            // Tell depth map to use yellow
        }
        else if (args.text.Equals("Set Color Yellow"))
        {
            DepthMapMaterial.SetInt("_Color", 4);

            // Tell depth map to use orange
        }
        else if (args.text.Equals("Set Color Orange"))
        {
            DepthMapMaterial.SetInt("_Color", 5);

            // Tell depth map to use purple
        }
        else if (args.text.Equals("Set Color Purple"))
        {
            DepthMapMaterial.SetInt("_Color", 6);

            // Tell depth map to use grey
        }
        else if (args.text.Equals("Set Color Grey"))
        {
            DepthMapMaterial.SetInt("_Color", 7);
        }

        // begin training
        else if (args.text.Equals("Begin Training"))
        {
            trialStageManager.trialStage = "begin_practice";
            print(trialStageManager.trialStage);
            eventManager.tempEvent = "Begin Training";
            mappingMaterial.DrawVisualMeshes = true;

        }

        // Tell experiment to start
        else if (args.text.Equals("Begin Trial"))
        {
            //DepthMapMaterial.SetInt("_Color", 7);
            eventManager.tempEvent = "Begin Trial";
            MainMenu.SetActive(false);

            // start data recording
            eventManager.enabled = true;
        }
        // Tell experiment to start
        else if (args.text.Equals("End Trial"))
        {
            // Stop drawing the meshes
            mappingMaterial.DrawVisualMeshes = false;

            // Bring back the menu
            MainMenu.SetActive(true);

            // stop rendering the cursor
            meshRenderer.enabled = false;

            eventManager.tempEvent = "End Trial";

            // stop data recording
            eventManager.enabled = false;
        }
    }
}
