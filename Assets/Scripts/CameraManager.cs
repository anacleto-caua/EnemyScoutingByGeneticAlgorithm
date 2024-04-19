using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{

    public Camera playerCamera;
    public Camera topDownCamera;

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.C))
        {
            if(playerCamera.enabled)
            {
                SwitchToTopDownCamera();
            }
            else
            {
                SwitchToPlayerCamera();
            }
        }
    }

    public void SwitchToTopDownCamera()
    {
        playerCamera.enabled = false;
        topDownCamera.enabled = true;
    }

    public void SwitchToPlayerCamera()
    {
        playerCamera.enabled = true;
        topDownCamera.enabled = false;
    }
}
