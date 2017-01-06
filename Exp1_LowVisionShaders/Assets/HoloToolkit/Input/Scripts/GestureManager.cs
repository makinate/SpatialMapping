// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using UnityEngine.VR.WSA.Input;

namespace HoloToolkit.Unity
{
    /// <summary>
    /// GestureManager creates a gesture recognizer and signs up for a tap gesture.
    /// When a tap gesture is detected, GestureManager uses GazeManager to find the game object.
    /// GestureManager then sends a message to that game object.
    /// </summary>
    [RequireComponent(typeof(GazeManager))]
    public partial class GestureManager : Singleton<GestureManager>
    {
        private GameObject Player;                      // Used to access the spatial mapping mesh
        SpatialMappingManager mappingMaterial;          // Represents the spatial mapping component
        private GestureRecognizer gestureRecognizer;

        void Start()
        {
            // Get a reference to the spatial mesh
            Player = GameObject.Find("SpatialMapping");
            mappingMaterial = Player.GetComponent<SpatialMappingManager>();

            // Create a new GestureRecognizer. Sign up for tapped events.
            gestureRecognizer = new GestureRecognizer();
            gestureRecognizer.SetRecognizableGestures(GestureSettings.Tap);

            gestureRecognizer.TappedEvent += GestureRecognizer_TappedEvent;

            // Start looking for gestures.
            gestureRecognizer.StartCapturingGestures();
        }

        private void GestureRecognizer_TappedEvent(InteractionSourceKind source, int tapCount, Ray headRay)
        {
            if (mappingMaterial.DrawVisualMeshes == false) {
                mappingMaterial.DrawVisualMeshes = true;
            } else {
                mappingMaterial.DrawVisualMeshes = false;
            }
        }

        void OnDestroy()
        {
            gestureRecognizer.StopCapturingGestures();
            gestureRecognizer.TappedEvent -= GestureRecognizer_TappedEvent;
        }
    }
}