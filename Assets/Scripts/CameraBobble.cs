using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
--- This code has has been written by Joshua Thompson (https://joshgames.co.uk) ---
        --- Copyright ©️ 2024-2025 AstralCandle Games. All Rights Reserved. ---
*/

public class CameraBobble : MonoBehaviour{
    [SerializeField] bool doPivot = false;
    [SerializeField] Utilities.MinMaxF rotateSpeed = new Utilities.MinMaxF(15, 25);
    [SerializeField] Utilities.MinMaxF orthographicSizeMenu = new Utilities.MinMaxF(15, 25);
    [SerializeField] float sineWaveFrequency = 1;
    [SerializeField] float frequency, amplitude;    
    Vector3 cachedPosition;
    Camera cam;

    private void Awake(){
        cam = Camera.main;
        cachedPosition = transform.localPosition;
    }

    void LateUpdate(){
        transform.localPosition = cachedPosition + new Vector3(Mathf.Cos((Time.time * frequency) / 2) * amplitude, Mathf.Sin(Time.time * frequency) * amplitude);    

        if(doPivot){
            float value = Utilities.Remap(Mathf.Sin(Time.time * sineWaveFrequency), -1, 1, 0, 1);

            transform.parent.Rotate(Vector3.up, Mathf.Lerp(rotateSpeed.min, rotateSpeed.max, value) * Time.deltaTime);
            cam.orthographicSize = Mathf.Lerp(orthographicSizeMenu.min, orthographicSizeMenu.max, value);
        }
    }    
}
