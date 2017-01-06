using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine.Windows.Speech;
using HoloToolkit.Unity;

/// <summary>
/// overall experiment loop
/// goes through each stage of the experiment and controls the flow
/// </summary>
public class ExperimentLoop : MonoBehaviour
{
    public TextToSpeechManager textToSpeechManager;
    public int instr;
    public string trialStage;
    public int instrSwitch = 0;
    
    // Use this for initialization
    /// <summary>
    /// Used to get a reference to the Spatial Mapping Manager and shared material
    /// </summary>
    private GameObject Player;
    private GameObject MainMenu;
    private GameObject Camera;

    private string nextTrialStage;

    LogPosRot eventManager;

    /// <summary>
    /// We use a voice command to enable moving the target.
    KeywordRecognizer keywordRecognizer;

    void Start()
    {
        /// begin experiment loop
        
        // set trial stage
        trialStage = "initializing";
        trialStage = "wait_for_input"; // remove this later

        // Play start instructions
        instrSwitch = 1;
        PlayInstructions();

        // set current VoiceCommand to "Listening"
        Camera = GameObject.Find("Main Camera");
        eventManager = Camera.GetComponent<LogPosRot>();
        eventManager.tempEvent = "Start";


        print(eventManager.tempEvent);
        StartCoroutine(wait1());
        

        if (trialStage == "go_to_practiceRoom")
        {

            StartCoroutine(wait2());
            print("wait2 over; going to wait_for_input");

        }

        else if (trialStage == "wait_for_input")
        {
            print("waiting for user input");
        }
        else if (trialStage == "begin_practice")
        {
            StartCoroutine(wait3());
            print("wait3 over; going to practice");
        }
        else if (trialStage == "practice")
        {

            StartCoroutine(waitPractice());

        }
        else if (trialStage == "practice_over")
        {
            StopCoroutine(waitPractice());
            print("Practice is over. The time is " + DateTime.Now.ToString("HH:mm:ss"));

            trialStage = "prepare_task_chair";
        }
        else if (trialStage == "prepare_task_chair")
        {
            // give instructions to go to next room and say "Begin trial" whenever ready
            // sit idle and wait for user input: "Begin trial
        }
        else if (trialStage == "begin_task_chair")
        {
            /// pick shader and layout according to selection
            /// then go to task
            trialStage = "task_chair";
        }
        else if (trialStage == "task_chair")
        {
            // sit idle and wait for user input: "Found chair"
        }
        else if (trialStage == "end_task_chair")
        {
            /// give instructions to get up and explore for two minutes
            /// wait for voice command for exploring
        }
        else if (trialStage == "task_exploring")
        {
            // sit idle and wait two minutes
        }
        else if (trialStage == "end_task_chair")
        {
            /// give instructions to find experimenter
            /// close app
        }
    }
 

    private void PlayInstructions()
    {
        switch (instrSwitch)
        {
            case 0:
                {
                    print("Nothing to say!");
                    break;
                }
            case 1:
                {
                    textToSpeechManager.SpeakText("Welcome to our study! The time is " + DateTime.Now.ToString("t"));
                    break;
                }
            case 2:
                {
                    textToSpeechManager.SpeakText("Explore the room with the HoloLens");
                    break;
                }
            case 3:
                {
                    textToSpeechManager.SpeakText("Explore the room for two minutes.");
                    break;
                }
            default:
                {
                    print("Incorrect level; nothing to say");
                    break;
                }
        }
    }
    IEnumerator wait1()
    {
        yield return new WaitForSeconds(10);
        // move to next trial stage
        trialStage = "go_to_practiceRoom";
        print(trialStage);
    }
    IEnumerator wait2()
    {

        yield return new WaitForSeconds(10);
        instrSwitch = 2;
        PlayInstructions();
        instrSwitch = 0;
        // move to next trial stage
        trialStage = "wait_for_input";
    }
    IEnumerator wait3()
    {
        instrSwitch = 3;
        PlayInstructions();
        instrSwitch = 0;
        yield return new WaitForSeconds(10);
        trialStage = "practice";
    }

   IEnumerator waitPractice()
    {
        yield return new WaitForSeconds(5);
        trialStage = "practice_over";
    }

}