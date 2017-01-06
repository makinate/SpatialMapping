using UnityEngine;
using System.Collections;

// This class is used to pass the depth values of the Main Camera to the
// Depth Map shader used in the Depth Map material
public class PostProcessDepthGreyscale : MonoBehaviour {

    public Material DepthMapMaterial;

	// Use this for initialization
	void Start () {
        DepthMapMaterial.SetVector("_CameraPos", Camera.main.transform.position);
	}

    // Passes the depth texture to the material containing the DepthMap shader
    private void Update()
    {
        DepthMapMaterial.SetVector("_CameraPos", Camera.main.transform.position);
    }
}
