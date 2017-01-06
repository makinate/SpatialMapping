using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using HoloToolkit.Unity;

public class LoadOnTab : MonoBehaviour {

    private MeshRenderer meshRenderer;

    // Called by GazeGestureManager when the user performs a Select gesture
    void OnSelect()
    {
        // If the sphere has no Rigidbody component, add one to enable physics.
        if (!this.GetComponent<Rigidbody>())
        {
            //var rigidbody = this.gameObject.AddComponent<Rigidbody>();
            //rigidbody.collisionDetectionMode = CollisionDetectionMode.Continuous;
            gameObject.SetActive(false);

            // stop rendering the cursor
            meshRenderer.enabled = false;


        }
    }

}
