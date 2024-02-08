using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using AstralCandle.TowerDefence;

public class SelectionCircle : MonoBehaviour{
    public BaseEntity entity;
    Camera cam;

    Vector3 position, positionVelocity;
    [SerializeField] float smoothing;
    [SerializeField] Vector3 offset;
    [SerializeField] Spawn spawnSettings;
    [SerializeField] Oscillation oscillationSettings;
    [SerializeField] Circle healthCircle, attackCircle;

    private void OnValidate() {
        if(Application.isPlaying || !entity){ return; }
        position = entity.transform.position + offset;
        transform.position = position;
    }

    private void Start() {
        cam = Camera.main;
        position = entity.transform.position + offset;
        transform.position = position;
        spawnSettings.cachedScale = transform.localScale;

        HealthEntity hE = (HealthEntity)entity;
        healthCircle.percent = hE.GetHealthPercentage();
        healthCircle.UpdateBar(true);
        ShowCircles();
    }

    private void LateUpdate() {   
        spawnSettings.doEaseIn(transform, entity != null);   
        if(transform.localScale.sqrMagnitude <= 0){ Destroy(gameObject); } // Deletes itself  
        
        if(!entity){ return; }
        transform.position = GetEntityPosition();        
        transform.rotation = GetDirectionToCamera();

        HealthEntity hE = (HealthEntity)entity;
        healthCircle.percent = hE.GetHealthPercentage();
        healthCircle.UpdateBar();
    }

    /// <summary>
    /// Based on the entity, will display different circles (NEED TO IMPLEMENT TOWERS)
    /// </summary>
    void ShowCircles(){
        HealthEntity hE = (HealthEntity)entity;
        bool hasAttack = (CharacterEntity)entity != null;// || (TowerEntity)entity != null;

        healthCircle.circle.SetActive(hE != null);
        attackCircle.circle.SetActive(hasAttack);
    }

    /// <summary>
    /// Calculates the position for this circle
    /// </summary>
    /// <returns>The entity position + sin wave offset</returns>
    Vector3 GetEntityPosition(){
        position = Vector3.SmoothDamp(position, entity.transform.position + offset, ref positionVelocity, smoothing);
        float x = Mathf.Cos(Time.time * oscillationSettings.oscillationFrequency) * oscillationSettings.oscillationAmplitude;
        float y = Mathf.Sin(Time.time * oscillationSettings.oscillationFrequency / 2) * oscillationSettings.oscillationAmplitude;
        float z = Mathf.Sin(Time.time * oscillationSettings.oscillationFrequency) * oscillationSettings.oscillationAmplitude;
        return position + new Vector3(x, y, z);
    }

    /// <summary>
    /// Calculates the direction in order to face the camera on the Y-axis
    /// </summary>
    /// <returns>The rotation to make this object face the camera</returns>
    Quaternion GetDirectionToCamera(){
        Vector3 dir = transform.position - cam.transform.position;
        dir.y = 0;
        Quaternion lookDir = Quaternion.LookRotation(dir);
        return lookDir * Quaternion.Euler(90, 0, 0);
    }
   
    [Serializable] public sealed class Spawn{
        public AnimationCurve animation;
        public float duration = 0.5f;
        [HideInInspector] public Vector3 cachedScale;

        float elapsedTime = 0;

        /// <summary>
        /// Adds a bit of flare to spawning the selection circle in
        /// </summary>
        /// <param name="obj">This transform</param>
        /// <param name="easeIn">Are we spawning in this object?</param>
        public void doEaseIn(Transform obj, bool easeIn){
            elapsedTime += (easeIn)? Time.deltaTime: -Time.deltaTime;
            elapsedTime = Mathf.Clamp(elapsedTime, 0, duration);

            obj.localScale = Vector3.Lerp(Vector3.zero, cachedScale, animation.Evaluate(Mathf.Clamp01(elapsedTime / duration)));
        }
    }
    [Serializable] public sealed class Oscillation{
        public float oscillationFrequency = 5f, oscillationAmplitude = 0.025f;
    }
    [Serializable] public sealed class Circle{
        public GameObject circle;
        public Image[] bars, delayedBars;
        [SerializeField, Tooltip("The animation the circle will take when updating")] AnimationCurve animation;

        [SerializeField, Tooltip("The time it takes to update")] float barUpdateDuration = 0.5f;
        [SerializeField, Tooltip("How responsive is the delayed bar to match the main bar?")] float delayedBarSmoothing = 0.1f;
        float elapsedTime;

        float _percent;
        /// <summary>
        /// Updating this will trigger
        /// </summary>
        public float percent{
            get => _percent;
            set{
                if(value != _percent){ 
                    elapsedTime = 0; 
                    previousBarLerp = barLerp;
                }
                _percent = value;
            }
        }
        float previousBarLerp, barLerp;
        float delayedFill, delayedVelocity;

        /// <summary>
        /// Interpolates the bar to a percentage
        /// </summary>
        /// <param name="instant">If true will instantly teleport the bars to a given point</param>
        public void UpdateBar(bool instant = false){
            if(instant){ previousBarLerp = percent / bars.Length; }

            elapsedTime += Time.deltaTime;
            float barPercentTime = Mathf.Clamp01(elapsedTime / barUpdateDuration); 
            barPercentTime = animation.Evaluate(barPercentTime);

            barLerp = Mathf.LerpUnclamped(previousBarLerp, percent / bars.Length, barPercentTime);
            delayedFill = Mathf.SmoothDamp(delayedFill, barLerp, ref delayedVelocity, (instant || barLerp > delayedFill)? 0: delayedBarSmoothing);
            
            for(int i = 0; i < bars.Length; i++){
                bars[i].fillAmount = barLerp; 
                delayedBars[i].fillAmount = delayedFill;
            }
        }
    }
}
