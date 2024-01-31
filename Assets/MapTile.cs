using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapTile : MonoBehaviour{
    public bool isActive = true;
    [SerializeField] float duration;
    [SerializeField] AnimationCurve sizeGrowth;
    
    float elapsedTime = 0;

    private void LateUpdate() {
        
        elapsedTime += (isActive)? Time.deltaTime : -Time.deltaTime;
        elapsedTime = Mathf.Clamp(elapsedTime, 0, duration);

        if(!isActive && transform.localScale.sqrMagnitude <= 0){ Destroy(gameObject); }
        Vector3 size = Vector3.LerpUnclamped(Vector3.zero, Vector3.one, sizeGrowth.Evaluate(elapsedTime/duration));
        transform.localScale = size;        
    }
}
