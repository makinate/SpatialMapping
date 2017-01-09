﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Windows.Speech;
using HoloToolkit.Unity;
using System;
using UnityEngine.VR.WSA.Input;

namespace HoloToolkit.Unity
{
    /// <summary>
    /// GestureManager creates a gesture recognizer and signs up for a tap gesture.
    /// When a tap gesture is detected, GestureManager uses GazeManager to find the game object.
    /// GestureManager then sends a message to that game object.
    /// </summary>
    [RequireComponent(typeof(GazeManager))]
    public partial class ClickerManager : Singleton<ClickerManager>
    {
        public TextToSpeechManager textToSpeechManager;

        private GameObject Experiment;                      // Used to access the spatial mapping mesh
        ExperimentManager experimentManager;          // Represents the spatial mapping component
        TrainingManager trainingManager;
        private GestureRecognizer gestureRecognizer;

        void Start()
        {
            // Get a reference to the spatial mesh
            Experiment          = GameObject.Find("Experiment");
            experimentManager   = Experiment.GetComponent<ExperimentManager>();
            trainingManager     = Experiment.GetComponent<TrainingManager>();

            // Create a new GestureRecognizer. Sign up for tapped events.
            gestureRecognizer = new GestureRecognizer();
            gestureRecognizer.SetRecognizableGestures(GestureSettings.Tap);

            gestureRecognizer.TappedEvent += GestureRecognizer_TappedEvent;

            // Start looking for gestures.
            gestureRecognizer.StartCapturingGestures();
        }

        private void GestureRecognizer_TappedEvent(InteractionSourceKind source, int tapCount, Ray headRay)
        {
            // do something
            //textToSpeechManager.SpeakText("Click! The time is " + DateTime.Now.ToString("t") + "Tap number: " + tapCount);
            tapCount = tapCount + 1;
            experimentManager.ClickSwitch = true;
            trainingManager.ClickSwitch = true;
        }

        void OnDestroy()
        {
            gestureRecognizer.StopCapturingGestures();
            gestureRecognizer.TappedEvent -= GestureRecognizer_TappedEvent;
        }
    }
}