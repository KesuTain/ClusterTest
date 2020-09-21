using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float zoomSpeed = 20f;
    public Transform Camera;

    public Clusterator myClusterator;

    public float CurrentZoom = 0f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        CurrentZoom = Mathf.Abs(Camera.localPosition.z);
        if (Camera)
        {
            float scroll = Input.GetAxis("Mouse ScrollWheel");
            if (scroll != 0.0f)
            {
                if (myClusterator)
                    myClusterator.Main();
                Camera.localPosition = new Vector3(0, 0, Camera.localPosition.z + scroll * zoomSpeed);
            }
        }
    }
}
