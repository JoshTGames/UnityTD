using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PixelCameraCopyMain : MonoBehaviour{
    Camera mainCam, thisCam;
    void Start(){
        mainCam = Camera.main;
        thisCam = GetComponent<Camera>();
    }

    void LateUpdate(){
        transform.position = mainCam.transform.position;
        thisCam.orthographicSize = mainCam.orthographicSize;
    }
}
