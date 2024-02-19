using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraBobble : MonoBehaviour{
    [SerializeField] float frequency, amplitude;    
    Vector3 cachedPosition;

    private void Awake() => cachedPosition = transform.position;

    void LateUpdate(){
        transform.position = cachedPosition + new Vector3(Mathf.Cos((Time.time * frequency) / 2) * amplitude, Mathf.Sin(Time.time * frequency) * amplitude);    
    }    
}
