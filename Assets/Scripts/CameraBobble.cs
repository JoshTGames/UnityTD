using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
--- This code has has been written by Joshua Thompson (https://joshgames.co.uk) ---
        --- Copyright ©️ 2024-2025 AstralCandle Games. All Rights Reserved. ---
*/

public class CameraBobble : MonoBehaviour{
    [SerializeField] float frequency, amplitude;    
    Vector3 cachedPosition;

    private void Awake() => cachedPosition = transform.position;

    void LateUpdate(){
        transform.position = cachedPosition + new Vector3(Mathf.Cos((Time.time * frequency) / 2) * amplitude, Mathf.Sin(Time.time * frequency) * amplitude);    
    }    
}
