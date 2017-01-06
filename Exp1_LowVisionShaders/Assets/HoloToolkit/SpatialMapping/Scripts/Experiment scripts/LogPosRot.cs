using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine.Windows.Speech;
using HoloToolkit.Unity;



/// <summary>
// LogPosRot gets position, rotation, events, and timestamp and saves it to a file
/// </summary>
public class LogPosRot : MonoBehaviour
{
    public Dropdown dropdown;
    public Text studyInfo;
    public Text sbjInfo;    // subject name
    public Text shaderInfo;     // shader name: needs to be accessible through other scripts
    public Text layoutInfo;
    public string tempEvent;   // eventName: needs to be accessible through other scripts
    public GameObject SpatialMapping;
    public string study = "study_00";
    public string sbj = "sbj_00";
    public string layout = "layout_00";
    public string shader = "shader_00";

    public GameObject Experiment;


    private Text posText;    // needs some kind of text game object to display position information
    private PTDataFileWriter dataWriter = new PTDataFileWriter();
    private VoicePlacement voicePlacement = new VoicePlacement();
    private string filePath;
    private string fileName;     // file pathname
    private Vector3 tempPos;     // position
    private Vector3 tempRot;     // rotation
    private Vector3 tempPosRay;  // position
    private Vector3 tempNormRay; // rotation/surface normal
    private float tempTime;      // time + deltaTime
    private string timeStamp;
    private ExperimentManager experimentManager;



    void Start()
    {
        // define output path
        experimentManager = Experiment.GetComponent<ExperimentManager>();
        
        string timeStamp = string.Format("_{0:yyyy-MM-dd_hh-mm-ss-tt}", DateTime.Now);
        study  = studyInfo.text;
        sbj    = sbjInfo.text;
        layout = layoutInfo.text;
        shader = shaderInfo.text;

        Debug.Log("study: " + study);
        Debug.Log("sbj: " + sbj);
        Debug.Log("shader: " + layout);
        Debug.Log("shader: " + shader);

        filePath = "";
        fileName = study + '_' + sbj +'_'+ layout + '_' + shader + '_' + timeStamp;
        Debug.Log("fileName: " + fileName);


        // define variable values at start
        tempPos = transform.position;             // save position at start
        tempRot = transform.rotation.eulerAngles; // save rotation at start

        // Get Gaze Manager so we can access it's variables
        tempPosRay  = GazeManager.Instance.PosRay;
        tempNormRay = GazeManager.Instance.NormRay;
        
        tempTime   = 0;

        tempEvent = experimentManager.currentState.ToString();

        // create output file and write header
        dataWriter.CreateNewAppendableFile(string.Format("{0}{1}", filePath, fileName), false); // set timestamp = false!
        dataWriter.AppendStringToOpenedFile(string.Format("pos.x; pos.y; pos.z; rot.x; rot.y; rot.z; gaze.x; gaze.y; gaze.z; gazeNormal.x; gazeNormal.y; gazeNormal.z; event; time\r\n"));
        

    }


    void FixedUpdate()
    {
        // get position, rotation, and time at each frame
        tempPos     = transform.position;
        tempRot     = transform.rotation.eulerAngles;
        tempPosRay  = GazeManager.Instance.PosRay;
        tempNormRay = GazeManager.Instance.NormRay;
        tempTime    = tempTime + Time.fixedDeltaTime;

        tempEvent = experimentManager.currentState.ToString();
         
        // make sure to format strings correctly (events as integers and not floats!)
        string line = System.String.Format("{0,3:f3};{1,3:f3};{2,3:f3};{3,3:f3};{4,3:f3};{5,3:f3};{6,3:f3};{7,3:f3};{8,3:f3};{9,3:f3};{10,3:f3};{11,3:f3}; {12, 0:f0};{13,3:f3}\r\n",
                                            tempPos.x, tempPos.y, tempPos.z,
                                            tempRot.x, tempRot.y, tempRot.z,
                                            tempPosRay.x, tempPosRay.y, tempPosRay.z,
                                            tempNormRay.x, tempNormRay.y, tempRot.z,
                                            tempEvent, tempTime);
        // append position, rotation, and time at each frame to file;
        dataWriter.AppendStringToOpenedFile(string.Format(line));

        // Display the current information 
        //ShowPosRotText();

    }
    
    void OnApplicationQuit ()
    {
        // Close file when application quits
        dataWriter.CloseAppendableFile();
        
    }


    // makes a string from position info
    void ShowPosRotText()
    {
        posText.text = "Participant: "  + sbj +
                       "\r\nPosition: " + tempPos +
                       "\r\nRotation: " + tempRot +
                       "\r\nGaze Position: " + tempPosRay +
                       "\r\nGaze Normal: " + tempNormRay +
                       "\r\nTime: " + tempTime;
    }

    //TO DO: close file when experiment is over (use a different script for this
    // Close appendable file

}
