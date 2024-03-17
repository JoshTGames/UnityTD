using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using AstralCandle.Animation;
using UnityEngine.Analytics;

/*
--- This code has has been written by Joshua Thompson (https://joshgames.co.uk) ---
        --- Copyright ©️ 2024-2025 AstralCandle Games. All Rights Reserved. ---
*/

public class DamagePopup : MonoBehaviour{
    [SerializeField] TMP_Text label;
    [SerializeField] AnimationInterpolation easingSettings;
    [SerializeField] float yOffset;

    Camera cam;
    Vector3 cachedSize;
    Vector3 cachedPosition;

    /// <summary>
    /// Sets the text of this popup 
    /// </summary>
    public void SetText(string text) => label.text = text;

    /// <summary>
    /// Sets the colour of this popup 
    /// </summary>
    public void SetColour(Color colour) => label.color = colour;

    /// <summary>
    /// Calculates the direction in order to face the camera on the Y-axis
    /// </summary>
    /// <returns>The rotation to make this object face the camera</returns>
    Quaternion GetDirectionToCamera(){
        Vector3 dir = transform.position - cam.transform.position;
        Quaternion lookDir = Quaternion.LookRotation(dir);
        return lookDir;
    }

    private void Start(){
        cam = Camera.main;
        cachedSize = transform.localScale;
        cachedPosition = transform.position;
        easingSettings.ResetTime();
    }

    private void LateUpdate() {
        float value = easingSettings.Play();
        Color curColor = label.color;
        label.color = new Color(curColor.r, curColor.g, curColor.b, value);
        transform.localScale = Vector3.LerpUnclamped(Vector3.zero, cachedSize, value);
        transform.position = Vector3.LerpUnclamped(cachedPosition, cachedPosition + new Vector3(0, yOffset), easingSettings.percent);

        if(easingSettings.percent >= 1){
            Destroy(gameObject);
            return;
        }

        transform.rotation = GetDirectionToCamera();
    }        
}
