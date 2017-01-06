using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.Windows.Speech;
using HoloToolkit.Unity;
using System;
using UnityEngine.VR.WSA.Input;

public class MeshManager : MonoBehaviour {

    public GameObject Layout1;
    public GameObject Layout2;
    public GameObject Layout3;
    public GameObject Layout4;

    public Text layoutInfo;

    private GameObject SpatialMapping;
    private ObjectSurfaceObserver objectSurfaceObserver;
    private string layout = "layout_00";
    // Use this for initialization
    void Start () {
        
        // Get a reference to the spatial mesh
        SpatialMapping = GameObject.Find("SpatialMapping");
        objectSurfaceObserver = SpatialMapping.GetComponent<ObjectSurfaceObserver>();

        layout = layoutInfo.text;

    }
}
