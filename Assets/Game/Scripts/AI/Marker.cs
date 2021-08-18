using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Marker : MonoBehaviour
{
    [Min(0)]
    public float scale = 0.005f;


    // Update is called once per frame
    void Update()
    {
        Transform cameraTransform = Camera.main.transform;

        // Look to camera
        transform.LookAt(cameraTransform.position);

        // Set the size depending from the distance to the camera
        float sc = scale * Vector3.Distance(cameraTransform.position, this.transform.position);
        this.transform.localScale = new Vector3(sc, sc, sc);
    }
}
